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
    public async Task CreateCampaignCommand_CorrectlyCreatesCampaign()
    {
        // Arrange
        string campaignName = "NewTestCampaign";
        string campaignDescription = "TestCampaignDescription";
        string campaignImageUrl = "TestCampaignImageUrl.jpg";
        
        CreateCampaignCommand createCampaignCommand = new(campaignName, campaignDescription, campaignImageUrl);
        
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
}