namespace InventoryERP.Domain.Entities;

public class TreasuryTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string TransactionType { get; set; } = string.Empty; // Receipt, Payment, Expense, Income, Sale, Purchase
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ReferenceNumber { get; set; } // Invoice number or payment reference
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }

    public int? PaymentId { get; set; }
    public int? ExpenseId { get; set; }
    public Payment? Payment { get; set; }
    public Expense? Expense { get; set; }
}
