using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class NpcResourceModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static NpcResourceModel From(NpcResource npc) => new()
    {
        Id = npc.Id,
        Name = npc.Name,
        Role = npc.Role,
        Notes = npc.Notes
    };

    public NpcResource ToValueObject() => new(Id, Name, Role, Notes);

    public void Apply(NpcResourceModel source)
    {
        Name = source.Name;
        Role = source.Role;
        Notes = source.Notes;
    }
}
