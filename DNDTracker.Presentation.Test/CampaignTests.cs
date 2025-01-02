using DNDTracker.Domain.Entities;
using DNDTracker.Domain.Exceptions;
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
        string campaignImageUrl = "testimageurl.png";
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
    
    [Fact]
    public void When_CreatingCampaign_WithInvalidData_Then_CampaignNotCreated()
    {
        // *** Arrange
        string name = "";
        string description = "";
        string campaignImageUrl = "";
        bool isActive = true;
        
        // *** Act
        Action creationAction = () => Campaign.Create(
            name,
            description,
            campaignImageUrl,
            isActive);
        
        // *** Assert
        creationAction.Should().Throw<InvalidCampaignDataException>();
    }
}