namespace InventoryERP.Domain.Entities;

public class CashMovement : BaseEntity
{
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public int CashSafeId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OperationType { get; set; } = string.Empty;

    public CashSafe? CashSafe { get; set; }
}
