namespace InventoryERP.Domain.Entities;

public class CashSafe : BaseEntity
{
    public string Name { get; set; } = "الخزنة الأساسية";
    public decimal CurrentBalance { get; set; }
}
