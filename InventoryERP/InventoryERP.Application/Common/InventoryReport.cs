using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Common;

public class InventoryReport
{
    public int TotalItems { get; set; }
    public decimal TotalStockValue { get; set; }
    public List<Item> LowStockItems { get; set; } = new();
    public List<StockMovement> RecentMovements { get; set; } = new();
}
