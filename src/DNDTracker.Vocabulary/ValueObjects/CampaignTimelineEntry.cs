namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CampaignTimelineEntry(
    Guid Id,
    DateTime OccurredAt,
    string Description);
