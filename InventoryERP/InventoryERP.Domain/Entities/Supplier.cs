namespace InventoryERP.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string? Notes { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalPayments { get; set; }

    public decimal CurrentBalance => OpeningBalance + TotalPurchases - TotalPayments;

    public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
