namespace DNDTracker.Domain.Campaigns;

public interface ICampaignRepository
{
    Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken);
    Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken);
    Task UpdateAsync(CancellationToken cancellationToken);
}