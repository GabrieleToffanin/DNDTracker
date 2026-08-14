using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddCampaignMemberRequest(
    Guid UserId,
    string DisplayName,
    CampaignMemberRole Role);
