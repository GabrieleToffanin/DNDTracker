using System.Net.Http.Json;
using DNDTracker.Application.Responses;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.IntegrationTests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;

namespace DNDTracker.IntegrationTests.CampaignsTests;

[Collection( "IntegrationTest" )]
public class CampaignIntegrationTests(IntegrationTestEnvironment testEnvironment)
    : IClassFixture<IntegrationTestEnvironment>
{
    private readonly HttpClient _client = testEnvironment.CreateClient();

    [Fact]
    [Trait( "Category", "Integration" )]
    public async Task CreateCampaignCommand_CorrectlyCreatesCampaign()
    {
        // Arrange
        string campaignName = "NewTestCampaign";
        string campaignDescription = "TestCampaignDescription";
        string campaignImageUrl = "TestCampaignImageUrl.jpg";
        
        CreateCampaignCommand createCampaignCommand =
            new(campaignName, campaignDescription, campaignImageUrl, DateTime.UtcNow);
        
        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/campaign",
            createCampaignCommand);
        
        response.EnsureSuccessStatusCode();
        
        var getResponse = await _client.GetAsync($"/api/campaign/{campaignName}");
        
        //Assert
        response.EnsureSuccessStatusCode();
        var campaign = JsonConvert.DeserializeObject<CampaignDto>(await getResponse.Content.ReadAsStringAsync());
        
        campaign.Should().NotBeNull();
    }
    
    [Fact]
    [Trait( "Category", "Integration" )]
    public async Task GetCampaignQuery_ReturnsCorrectCampaign()
    {
        // Arrange
        string campaignName = "TestCampaign";
        
        // Act
        var response = await _client.GetAsync($"/api/campaign/{campaignName}");
        
        // Assert
        response.EnsureSuccessStatusCode(); // StatusCode should be 200

        var campaign = JsonConvert.DeserializeObject<CampaignDto>(await response.Content.ReadAsStringAsync());
        campaign.Should().NotBeNull();
    }
}