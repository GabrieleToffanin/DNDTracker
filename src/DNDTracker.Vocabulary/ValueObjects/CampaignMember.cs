using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CampaignMember(
    Guid UserId,
    string DisplayName,
    CampaignMemberRole Role);
