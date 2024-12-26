using DNDTracker.Domain.Entities;
using DNDTracker.Presentation.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using FluentAssertions;

namespace DNDTracker.Presentation.IntegrationTests;

public class GetCampaignQueryIntegrationTest : IClassFixture<IntegrationTestEnvironment>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestEnvironment _testEnvironment;

    public GetCampaignQueryIntegrationTest(IntegrationTestEnvironment testEnvironment)
    {
        // Arrange
        _testEnvironment = testEnvironment;
        _client = _testEnvironment.CreateClient();
    }

    [Trait( "Category", "Integration" )]
    [Theory(Skip = "Too much config for a simple thing on Actions :(")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task GetCampaignQuery_ReturnsCorrectCampaign(string campaignId)
    {
        // Arrange
        Guid id = Guid.Parse(campaignId);
        
        // Act
        var response = await _client.GetAsync($"/api/campaigns/{id}");
        
        // Assert
        response.EnsureSuccessStatusCode(); // StatusCode should be 200

        var campaign = JsonConvert.DeserializeObject<Campaign>(await response.Content.ReadAsStringAsync());
        campaign.Should().NotBeNull();
    }
}