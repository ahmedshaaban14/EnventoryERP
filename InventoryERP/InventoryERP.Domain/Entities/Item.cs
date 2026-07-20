namespace InventoryERP.Domain.Entities;

public class Item : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public int? CategoryId { get; set; }

    public string Brand { get; set; } = string.Empty;
    public string Car { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string Unit { get; set; } = "قطعة";
    public int MinimumStock { get; set; }
    // Backward compatibility.
    public string StorageLocation { get => Rack; set => Rack = value; }

    public string Rack { get; set; } = string.Empty;

    public string Shelf { get; set; } = string.Empty;
    // Backward compatibility with old prototype code/tests.
    public decimal PurchasePrice { get => LastPurchasePrice; set => LastPurchasePrice = value; }

    public decimal LastPurchasePrice { get; set; }

    public decimal AverageCost { get; set; }
    public decimal SalePrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public int CurrentStock { get; set; }
    public string? ImagePath { get; set; }
    // Backward compatibility.
    public string? Description { get => Notes; set => Notes = value; }

    public string? Notes { get; set; }


    public Category? Category { get; set; }
    public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();
    public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
