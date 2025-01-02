using DNDTracker.Application.Responses;
using DNDTracker.Domain.Entities;
using DNDTracker.Presentation.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using FluentAssertions;

namespace DNDTracker.Presentation.IntegrationTests;

public class GetCampaignQueryIntegrationTest(IntegrationTestEnvironment testEnvironment)
    : IClassFixture<IntegrationTestEnvironment>
{
    private readonly HttpClient _client = testEnvironment.CreateClient();

    [Trait( "Category", "Integration" )]
    [Theory]
    [InlineData("TestCampaign")]
    public async Task GetCampaignQuery_ReturnsCorrectCampaign(string campaignName)
    {
        // Arrange
        
        
        // Act
        var response = await _client.GetAsync($"/api/campaign/{campaignName}");
        
        // Assert
        response.EnsureSuccessStatusCode(); // StatusCode should be 200

        var campaign = JsonConvert.DeserializeObject<CampaignDto>(await response.Content.ReadAsStringAsync());
        campaign.Should().NotBeNull();
    }
}