namespace InventoryERP.Domain.Entities;

public class AccountTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }

    public Customer? Customer { get; set; }
    public Supplier? Supplier { get; set; }
}
