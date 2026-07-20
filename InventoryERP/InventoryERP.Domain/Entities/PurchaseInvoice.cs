namespace InventoryERP.Domain.Entities;

public class PurchaseInvoice : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public string? Warehouse { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Credit, Transfer
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? Notes { get; set; }
    public bool IsCanceled { get; set; }
    public DateTime? CanceledDate { get; set; }
    public string? CanceledBy { get; set; }

    public Supplier? Supplier { get; set; }
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
