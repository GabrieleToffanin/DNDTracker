using DNDTracker.Domain.Campaigns;
using FluentAssertions;

namespace DNDTracker.Application.Tests.Behaviors.Dummies;

public class DummyCampaignRepository : ICampaignRepository
{
    public Dictionary<string, Campaign> Campaigns { get; } = new();
    
    public async Task<Campaign?> GetCampaignAsync(string campaignName, CancellationToken cancellationToken)
    {
        return Campaigns.Values.FirstOrDefault(c => c.CampaignName == campaignName);
    }

    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CancellationToken cancellationToken)
    {
        return Campaigns.Values;
    }

    public async Task CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        Insert(campaign);
    }

    public async Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        //Values will be updated by ref in this case
    }

    internal void Insert(Campaign campaign) => Campaigns[campaign.CampaignName] = campaign;
    
    internal void AssertCampaignEquals(Campaign expectedCampaign)
    {
        var existingCampaign = Campaigns[expectedCampaign.CampaignName];
        existingCampaign.CampaignName.Should().Be(expectedCampaign.CampaignName);
        existingCampaign.CampaignDescription.Should().Be(expectedCampaign.CampaignDescription);
        existingCampaign.CampaignImage.Should().Be(expectedCampaign.CampaignImage);
        existingCampaign.IsActive.Should().Be(expectedCampaign.IsActive);
        existingCampaign.CreatedDate.Should().Be(expectedCampaign.CreatedDate);
        existingCampaign.UpdatedDate.Should().Be(expectedCampaign.UpdatedDate);
        existingCampaign.DeletedDate.Should().Be(expectedCampaign.DeletedDate);
        existingCampaign.Heroes.Should().BeEquivalentTo(expectedCampaign.Heroes);
    }
}