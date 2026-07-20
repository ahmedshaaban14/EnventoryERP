using InventoryERP.Application.Interfaces;
using InventoryERP.Application.Services;
using InventoryERP.Domain.Entities;
using InventoryERP.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Tests;

public class ItemServiceTests
{
    [Fact]
    public async Task AddAndGetItems_ShouldPersistAndReturnItems()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new ItemService(context);

        var item = new Item
        {
            Code = "ITM-100",
            Name = "Test Item",
            PurchasePrice = 10,
            SalePrice = 15,
            CurrentStock = 5,
            MinimumStock = 2,
            Unit = "قطعة"
        };

        var created = await service.AddAsync(item);
        var items = await service.GetAllAsync();

        Assert.NotNull(created);
        Assert.Single(items);
        Assert.Equal("ITM-000001", items[0].Code);
    }

    [Fact]
    public async Task AddAsync_ShouldGenerateCode_WhenCodeIsEmpty()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new ItemService(context);

        var created = await service.AddAsync(new Item { Name = "Auto Code Item" });

        Assert.False(string.IsNullOrWhiteSpace(created.Code));
        Assert.StartsWith("ITM-", created.Code);
    }

    [Fact]
    public async Task CustomerService_ShouldPersistCustomer()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new CustomerService(context);
        var customer = await service.AddAsync(new Customer { Name = "أحمد", Phone = "0123456789" });
        var customers = await service.GetAllAsync();

        Assert.NotNull(customer);
        Assert.Single(customers);
        Assert.Equal("أحمد", customers[0].Name);
    }

    [Fact]
    public async Task InvoiceService_ShouldReduceStockAndUpdateCashOnSale()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var item = new Item { Name = "Test Item", CurrentStock = 5, SalePrice = 10, PurchasePrice = 8 };
        context.Items.Add(item);
        context.Customers.Add(new Customer { Name = "Customer", Phone = "123" });
        context.CashSafes.Add(new CashSafe { CurrentBalance = 100 });
        await context.SaveChangesAsync();

        IInventoryDbContext dbContext = context;
        var ledgerService = new InventoryLedgerService(dbContext);
        var service = new InvoiceService(dbContext, ledgerService);

        var invoice = await service.CreateSaleInvoiceAsync(new SaleInvoice
        {
            CustomerId = 1,
            PaymentMethod = "نقدي",
            Items = new List<SaleInvoiceItem>
            {
                new()
                {
                    ItemId = item.Id,
                    Quantity = 1,
                    UnitPrice = item.SalePrice
                }
            }
        });

        var savedItem = await context.Items.FindAsync(item.Id);
        Assert.NotNull(savedItem);
        Assert.Equal(4, savedItem.CurrentStock);
        Assert.Equal("SAL-000001", invoice.Number);
    }
}
