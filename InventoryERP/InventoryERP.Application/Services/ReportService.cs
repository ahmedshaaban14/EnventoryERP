using InventoryERP.Application.Common;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class ReportService : IReportService
{
    private readonly IInventoryDbContext _context;

    public ReportService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryReport> GetInventoryReportAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.Items
            .Where(x => x.IsActive)
            .OrderBy(x => x.CurrentStock)
            .Take(10)
            .ToListAsync(cancellationToken);

        var lowStockItems = items
            .Where(x => x.CurrentStock <= x.MinimumStock)
            .ToList();

        var recentMovements = await _context.StockMovements
            .OrderByDescending(x => x.MovementDate)
            .Take(10)
            .Include(x => x.Item)
            .ToListAsync(cancellationToken);

        return new InventoryReport
        {
            LowStockItems = lowStockItems,
            RecentMovements = recentMovements,
            TotalItems = items.Count,
            TotalStockValue = items.Sum(x => x.CurrentStock * x.SalePrice)
        };
    }
}
