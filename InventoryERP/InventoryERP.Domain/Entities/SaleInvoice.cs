namespace InventoryERP.Domain.Entities;

public class SaleInvoice : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Profit { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Credit, Transfer
    public string PaymentStatus { get; set; } = "غير مدفوع"; // Paid, Partial, Unpaid
    public string? Notes { get; set; }
    public bool IsCanceled { get; set; }
    public DateTime? CanceledDate { get; set; }
    public string? CanceledBy { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<SaleInvoiceItem> Items { get; set; } = new List<SaleInvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
