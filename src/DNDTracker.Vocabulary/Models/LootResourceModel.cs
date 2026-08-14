using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class LootResourceModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsMagicItem { get; set; }
    public string Notes { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static LootResourceModel From(LootResource loot) => new()
    {
        Id = loot.Id,
        Name = loot.Name,
        IsMagicItem = loot.IsMagicItem,
        Notes = loot.Notes
    };

    public LootResource ToValueObject() => new(Id, Name, IsMagicItem, Notes);

    public void Apply(LootResourceModel source)
    {
        Name = source.Name;
        IsMagicItem = source.IsMagicItem;
        Notes = source.Notes;
    }
}
