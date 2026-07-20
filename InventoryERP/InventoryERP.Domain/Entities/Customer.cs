namespace InventoryERP.Domain.Entities;

public class Customer : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Car { get; set; }
    public string? Notes { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public decimal TotalPurchases { get; set; }

    public decimal CurrentBalance => OpeningBalance + TotalSales - TotalPayments;

    public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
