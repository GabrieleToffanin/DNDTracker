using System.Net.Http.Json;
using DNDTracker.Application.Responses;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Presentation.IntegrationTests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;

namespace DNDTracker.Presentation.IntegrationTests.CampaignsTests;

public class CreateCampaignCommandIntegrationTest(IntegrationTestEnvironment testEnvironment)
    : IClassFixture<IntegrationTestEnvironment>
{
    private readonly HttpClient _client = testEnvironment.CreateClient();

    [Fact]
    [Trait( "Category", "Integration" )]
    public async Task GetCampaignQuery_ReturnsCorrectCampaign()
    {
        // Arrange
        string campaignName = "TestCampaign";
        string campaignDescription = "TestCampaignDescription";
        string campaignImageUrl = "TestCampaignImageUrl.jpg";
        
        CreateCampaignCommand createCampaignCommand = new(campaignName, campaignDescription, campaignImageUrl);
        
        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/campaign",
            createCampaignCommand);
        
        // Assert
        response.EnsureSuccessStatusCode(); // StatusCode should be 200

        var campaign = JsonConvert.DeserializeObject<CampaignDto>(await response.Content.ReadAsStringAsync());
        campaign.Should().NotBeNull();
    }
}