using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace InventoryERP.Application.Interfaces;

public interface IInventoryDbContext
{
    DbSet<Item> Items { get; }
    DbSet<Category> Categories { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseInvoice> PurchaseInvoices { get; }
    DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; }
    DbSet<SaleInvoice> SaleInvoices { get; }
    DbSet<SaleInvoiceItem> SaleInvoiceItems { get; }
    DbSet<StockTransaction> StockTransactions { get; }
    // Legacy: some services use StockMovements (older entity name). Keep both until full redesign.
    DbSet<StockMovement> StockMovements { get; }

    DbSet<TreasuryTransaction> TreasuryTransactions { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<AccountTransaction> AccountTransactions { get; }
    DbSet<CashSafe> CashSafes { get; }
    DbSet<CashMovement> CashMovements { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
