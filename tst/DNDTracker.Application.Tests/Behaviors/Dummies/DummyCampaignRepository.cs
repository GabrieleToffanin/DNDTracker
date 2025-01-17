using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;

namespace DNDTracker.Application.Tests.Behaviors.Dummies;

public class DummyCampaignRepository : ICampaignRepository
{
    public Dictionary<string, Campaign> Campaigns { get; } = new();
    
    public Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        Insert(campaign);
    }
    
    private void Insert(Campaign campaign) => Campaigns[campaign.Id.ToString()] = campaign;
}