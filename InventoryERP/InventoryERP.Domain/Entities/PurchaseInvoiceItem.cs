namespace InventoryERP.Domain.Entities;

public class PurchaseInvoiceItem : BaseEntity
{
    public int PurchaseInvoiceId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public PurchaseInvoice? PurchaseInvoice { get; set; }
    public Item? Item { get; set; }
}
