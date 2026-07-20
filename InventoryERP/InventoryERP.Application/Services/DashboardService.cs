using InventoryERP.Application.Common;
using InventoryERP.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IInventoryDbContext _context;

    public DashboardService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardOverview> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var todayStart = now.Date;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var items = await _context.Items
            .AsNoTracking()
            .Where(item => item.IsActive)
            .Include(item => item.Category)
            .ToListAsync(cancellationToken);

        var customers = await _context.Customers.AsNoTracking().Where(customer => customer.IsActive).ToListAsync(cancellationToken);
        var suppliers = await _context.Suppliers.AsNoTracking().Where(supplier => supplier.IsActive).ToListAsync(cancellationToken);
        var cashSafes = await _context.CashSafes.AsNoTracking().ToListAsync(cancellationToken);

        var sales = await _context.SaleInvoices
            .AsNoTracking()
            .Where(invoice => !invoice.IsCanceled)
            .Include(invoice => invoice.Items)
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync(cancellationToken);

        var purchases = await _context.PurchaseInvoices
            .AsNoTracking()
            .Where(invoice => !invoice.IsCanceled)
            .Include(invoice => invoice.Items)
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync(cancellationToken);

        var stockMovements = await _context.StockMovements
            .AsNoTracking()
            .Include(movement => movement.Item)
            .OrderByDescending(movement => movement.MovementDate)
            .Take(20)
            .ToListAsync(cancellationToken);

        var todaysSales = sales.Where(invoice => invoice.InvoiceDate >= todayStart).Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));
        var monthlySales = sales.Where(invoice => invoice.InvoiceDate >= monthStart).Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));
        var todaysPurchases = purchases.Where(invoice => invoice.InvoiceDate >= todayStart).Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));
        var monthlyPurchases = purchases.Where(invoice => invoice.InvoiceDate >= monthStart).Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));
        var inventoryValue = items.Sum(item => item.CurrentStock * item.AverageCost);
        var profit = monthlySales - monthlyPurchases;

        var monthlySalesSeries = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var monthDate = new DateTime(now.Year, now.Month, 1).AddMonths(-offset);
                var monthLabel = monthDate.ToString("MMM");
                var total = sales.Where(invoice => invoice.InvoiceDate.Year == monthDate.Year && invoice.InvoiceDate.Month == monthDate.Month)
                    .Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));

                return new MonthlySeriesPoint { Label = monthLabel, Value = total };
            })
            .Reverse()
            .ToList();

        var monthlyPurchasesSeries = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var monthDate = new DateTime(now.Year, now.Month, 1).AddMonths(-offset);
                var monthLabel = monthDate.ToString("MMM");
                var total = purchases.Where(invoice => invoice.InvoiceDate.Year == monthDate.Year && invoice.InvoiceDate.Month == monthDate.Month)
                    .Sum(invoice => invoice.Items.Sum(item => item.Quantity * item.UnitPrice));

                return new MonthlySeriesPoint { Label = monthLabel, Value = total };
            })
            .Reverse()
            .ToList();

        var distribution = new List<InventoryDistributionPoint>
        {
            new() { Label = "مخزون متاح", Value = items.Where(item => item.CurrentStock > item.MinimumStock).Sum(item => item.CurrentStock) },
            new() { Label = "قارب النفاد", Value = items.Count(item => item.CurrentStock > 0 && item.CurrentStock <= item.MinimumStock) },
            new() { Label = "نفد", Value = items.Count(item => item.CurrentStock <= 0) }
        };

        var activities = stockMovements
            .Select(movement => $"{movement.MovementType} - {movement.Item?.Name} - {movement.MovementDate:g}")
            .Take(10)
            .ToList();

        return new DashboardOverview
        {
            CompanyName = "Inventory ERP",
            CurrentUser = "مدير النظام",
            CurrentDateTime = now,
            TotalItems = items.Count,
            TotalCustomers = customers.Count,
            TotalSuppliers = suppliers.Count,
            InventoryValue = inventoryValue,
            TodaysSales = todaysSales,
            MonthlySales = monthlySales,
            TodaysPurchases = todaysPurchases,
            MonthlyPurchases = monthlyPurchases,
            CashBalance = cashSafes.Sum(safe => safe.CurrentBalance),
            NetProfit = profit,
            LowStockItems = items.Where(item => item.CurrentStock <= item.MinimumStock).OrderBy(item => item.Name).Take(10).ToList(),
            RecentSales = sales.Take(10).ToList(),
            RecentPurchases = purchases.Take(10).ToList(),
            RecentActivities = activities,
            MonthlySalesSeries = monthlySalesSeries,
            MonthlyPurchasesSeries = monthlyPurchasesSeries,
            InventoryDistribution = distribution
        };
    }
}
