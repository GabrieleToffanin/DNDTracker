using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class InventoryItemModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static InventoryItemModel From(InventoryItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Quantity = item.Quantity,
        Notes = item.Notes
    };

    public InventoryItem ToValueObject() => new(Id, Name, Quantity, Notes);

    public void Apply(InventoryItemModel source)
    {
        Name = source.Name;
        Quantity = source.Quantity;
        Notes = source.Notes;
    }
}
