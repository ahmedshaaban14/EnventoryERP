namespace InventoryERP.Domain.Entities;

public class SaleInvoiceItem : BaseEntity
{
    public int SaleInvoiceId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public SaleInvoice? SaleInvoice { get; set; }
    public Item? Item { get; set; }
}
