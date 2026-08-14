using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class CampaignTimelineEntryModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static CampaignTimelineEntryModel From(CampaignTimelineEntry entry) => new()
    {
        Id = entry.Id,
        OccurredAt = entry.OccurredAt,
        Description = entry.Description
    };

    public CampaignTimelineEntry ToValueObject() => new(Id, OccurredAt, Description);

    public void Apply(CampaignTimelineEntryModel source)
    {
        OccurredAt = source.OccurredAt;
        Description = source.Description;
    }
}
