namespace InventoryERP.Domain.Entities;

public class StockMovement : BaseEntity
{
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public int ItemId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public int QuantityBefore { get; set; }
    public int QuantityChange { get; set; }
    public int QuantityAfter { get; set; }
    public string Source { get; set; } = string.Empty;

    public Item? Item { get; set; }
}
