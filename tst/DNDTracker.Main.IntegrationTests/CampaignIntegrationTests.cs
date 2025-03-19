using System.Net;
using System.Net.Http.Json;
using DNDTracker.Main.IntegrationTests.Fixtures;
using DNDTracker.SDK.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DNDTracker.Main.IntegrationTests;

[Collection("Integration Tests")]
public class CampaignIntegrationTests(MainIntegrationTestsFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task CreateCampaign_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var createDate = DateTime.UtcNow;
        var campaign = new
        {
            CampaignName = "Test Campaign",
            CampaignDescription = "Test Description",
            CampaignImage = "test-image.jpg",
            CreateDate = createDate
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Campaign", campaign);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var location = response.Headers.Location?.ToString();
        location.Should().NotBeNullOrEmpty();
        
        var getResponse = await _client.GetAsync(location);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await getResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetCampaignResponse>(content);
        
        result.Should().NotBeNull();
        result?.CampaignName.Should().Be(campaign.CampaignName);
        result?.CampaignDescription.Should().Be(campaign.CampaignDescription);
    }
}