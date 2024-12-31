using DNDTracker.Domain.Entities;

namespace DNDTracker.Application.Abstractions;

public interface ICampaignRepository
{
    Task<Campaign> GetCampaignAsync(string campaignName);
}