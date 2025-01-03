using DNDTracker.Domain.Entities;

namespace DNDTracker.Domain.Abstractions;

public interface ICampaignRepository
{
    Task<Campaign> GetCampaignAsync(string campaignName);
}