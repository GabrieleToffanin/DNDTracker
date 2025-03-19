namespace DNDTracker.SDK.Responses;

public record GetCampaignResponse(
    string CampaignName,
    string CampaignDescription);