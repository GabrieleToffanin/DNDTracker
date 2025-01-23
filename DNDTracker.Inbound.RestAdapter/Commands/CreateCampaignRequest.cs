namespace DNDTracker.Inbound.RestAdapter.Commands;

public record CreateCampaignRequest(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    DateTime CreatedDate,
    bool IsActive = true);