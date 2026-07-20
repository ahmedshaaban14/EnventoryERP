using InventoryERP.Application.Services;
using InventoryERP.Domain.Entities;
using InventoryERP.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Tests;

public class ReportServiceTests
{
    [Fact]
    public async Task GetInventoryReportAsync_ShouldReturnLowStockAndRecentMovements()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.Items.Add(new Item
        {
            Code = "ITM-001",
            Name = "Low Stock Item",
            CurrentStock = 2,
            MinimumStock = 5,
            PurchasePrice = 10,
            SalePrice = 15
        });

        context.Items.Add(new Item
        {
            Code = "ITM-002",
            Name = "Healthy Item",
            CurrentStock = 10,
            MinimumStock = 5,
            PurchasePrice = 8,
            SalePrice = 12
        });

        context.StockMovements.Add(new StockMovement
        {
            ItemId = 1,
            MovementType = "بيع",
            QuantityBefore = 3,
            QuantityChange = -1,
            QuantityAfter = 2,
            Source = "SaleInvoice"
        });

        await context.SaveChangesAsync();

        var service = new ReportService(context);
        var report = await service.GetInventoryReportAsync();

        Assert.Single(report.LowStockItems);
        Assert.Equal("Low Stock Item", report.LowStockItems[0].Name);
        Assert.Single(report.RecentMovements);
    }
}
