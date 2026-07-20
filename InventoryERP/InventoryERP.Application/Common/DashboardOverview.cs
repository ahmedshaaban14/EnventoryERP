using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Common;

public class DashboardOverview
{
    public string CompanyName { get; set; } = "Inventory ERP";
    public string CurrentUser { get; set; } = "مدير النظام";
    public DateTime CurrentDateTime { get; set; } = DateTime.Now;

    public int TotalItems { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalSuppliers { get; set; }
    public decimal InventoryValue { get; set; }
    public decimal TodaysSales { get; set; }
    public decimal MonthlySales { get; set; }
    public decimal TodaysPurchases { get; set; }
    public decimal MonthlyPurchases { get; set; }
    public decimal CashBalance { get; set; }
    public decimal NetProfit { get; set; }

    public List<Item> LowStockItems { get; set; } = new();
    public List<SaleInvoice> RecentSales { get; set; } = new();
    public List<PurchaseInvoice> RecentPurchases { get; set; } = new();
    public List<string> RecentActivities { get; set; } = new();
    public List<MonthlySeriesPoint> MonthlySalesSeries { get; set; } = new();
    public List<MonthlySeriesPoint> MonthlyPurchasesSeries { get; set; } = new();
    public List<InventoryDistributionPoint> InventoryDistribution { get; set; } = new();
}

public class MonthlySeriesPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class InventoryDistributionPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
