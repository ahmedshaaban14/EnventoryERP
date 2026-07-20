namespace InventoryERP.Domain.Entities;

public class StockTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public int ItemId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Purchase, Sale, PurchaseReturn, SaleReturn, Adjustment, Count
    public int QuantityBefore { get; set; }
    public int QuantityChange { get; set; }
    public int QuantityAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? ReferenceNumber { get; set; } // Invoice number or adjustment reference
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }

    public Item? Item { get; set; }
}
