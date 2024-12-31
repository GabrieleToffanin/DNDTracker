using DNDTracker.Domain.Entities;
using FluentAssertions;

namespace DNDTracker.Presentation.Test;

[Collection( "CampaignTests")]
public sealed class CampaignTests
{
    [Fact]
    public async Task When_CreatingCampaign_WithValidData_Then_CampaignCreated()
    {
        // *** Arrange
        string name = "Test Campaign";
        string description = "Test Description";
        string campaignImageUrl = "Test Image Url";
        bool isActive = true;
        
        // *** Act
        Campaign campaign = Campaign.Create(
            name,
            description,
            campaignImageUrl,
            isActive);
        
        // *** Assert
        campaign.Should().NotBeNull();
        campaign.CampaignName.Should().Be(name);
        campaign.CampaignDescription.Should().Be(description);
        campaign.IsActive.Should().Be(isActive);
    }
}