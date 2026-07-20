namespace InventoryERP.Domain.Entities;

public class Payment : BaseEntity
{
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentType { get; set; } = string.Empty; // Customer, Supplier
    public int PartyId { get; set; } // CustomerId or SupplierId
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Bank, Transfer
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }

    public ICollection<TreasuryTransaction> TreasuryTransactions { get; set; } = new List<TreasuryTransaction>();
}
