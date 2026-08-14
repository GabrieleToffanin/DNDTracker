using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class CampaignMemberModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public CampaignMemberRole Role { get; set; }
    public CampaignModel Campaign { get; set; } = null!;

    public static CampaignMemberModel From(CampaignMember member) => new()
    {
        Id = Guid.NewGuid(),
        UserId = member.UserId,
        DisplayName = member.DisplayName,
        Role = member.Role
    };

    public CampaignMember ToValueObject() => new(UserId, DisplayName, Role);

    public void Apply(CampaignMemberModel source)
    {
        UserId = source.UserId;
        DisplayName = source.DisplayName;
        Role = source.Role;
    }
}
