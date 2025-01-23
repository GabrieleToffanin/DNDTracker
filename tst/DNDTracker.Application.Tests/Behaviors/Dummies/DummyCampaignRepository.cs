using DNDTracker.Domain.Campaigns;

namespace DNDTracker.Application.Tests.Behaviors.Dummies;

public class DummyCampaignRepository : ICampaignRepository
{
    public Dictionary<string, Campaign> Campaigns { get; } = new();
    
    public async Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        return Campaigns.Values.FirstOrDefault(c => c.CampaignName == campaignName);
    }

    public async Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        Insert(campaign);
    }
    
    private void Insert(Campaign campaign) => Campaigns[campaign.Id.ToString()] = campaign;
}