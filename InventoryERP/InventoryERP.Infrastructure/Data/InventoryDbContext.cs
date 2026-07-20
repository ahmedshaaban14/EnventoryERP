using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Infrastructure.Data;

public class InventoryDbContext : DbContext, IInventoryDbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<SaleInvoice> SaleInvoices => Set<SaleInvoice>();
    public DbSet<SaleInvoiceItem> SaleInvoiceItems => Set<SaleInvoiceItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public DbSet<TreasuryTransaction> TreasuryTransactions => Set<TreasuryTransaction>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();
    public DbSet<CashSafe> CashSafes => Set<CashSafe>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Barcode);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(32);
            entity.Property(e => e.Barcode).IsRequired().HasMaxLength(64);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EnglishName).HasMaxLength(200);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Car).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Rack).HasMaxLength(50);
            entity.Property(e => e.Shelf).HasMaxLength(50);
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.LastPurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.AverageCost).HasPrecision(18, 2);
            entity.Property(e => e.SalePrice).HasPrecision(18, 2);
            entity.Property(e => e.WholesalePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
            entity.Property(e => e.TotalSales).HasPrecision(18, 2);
            entity.Property(e => e.TotalPayments).HasPrecision(18, 2);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.TotalPurchases).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
            entity.Property(e => e.TotalPurchases).HasPrecision(18, 2);
            entity.Property(e => e.TotalPayments).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.Property(e => e.RemainingAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PurchaseInvoiceItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
        });


        modelBuilder.Entity<SaleInvoice>(entity =>
        {
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.Property(e => e.RemainingAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
            entity.Property(e => e.Profit).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SaleInvoiceItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
        });


        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasIndex(e => e.ItemId);
            entity.HasIndex(e => e.TransactionDate);
            entity.Property(e => e.UnitCost).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
            entity.HasOne(e => e.Item).WithMany(i => i.StockTransactions).HasForeignKey(e => e.ItemId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TreasuryTransaction>(entity =>
        {
            entity.HasIndex(e => e.TransactionDate);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceBefore).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.PartyId);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AccountTransaction>(entity =>
        {
            entity.Property(e => e.Debit).HasPrecision(18, 2);
            entity.Property(e => e.Credit).HasPrecision(18, 2);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CashSafe>(entity =>
        {
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CashMovement>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });
    }
}
