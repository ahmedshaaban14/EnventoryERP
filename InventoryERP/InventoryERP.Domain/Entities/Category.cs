namespace InventoryERP.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public override string ToString() => Name;


    public ICollection<Item> Items { get; set; } = new List<Item>();
}
