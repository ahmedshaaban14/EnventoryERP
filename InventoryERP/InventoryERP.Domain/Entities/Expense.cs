namespace InventoryERP.Domain.Entities;

public class Expense : BaseEntity
{
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public string ExpenseType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CreatedBy { get; set; }

    public ICollection<TreasuryTransaction> TreasuryTransactions { get; set; } = new List<TreasuryTransaction>();
}
