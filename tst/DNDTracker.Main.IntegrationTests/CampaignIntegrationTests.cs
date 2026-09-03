using System.Net;
using System.Net.Http.Json;
using DNDTracker.Main.IntegrationTests.Fixtures;
using DNDTracker.SDK.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DNDTracker.Main.IntegrationTests;

[Collection("Integration Tests")]
[Trait("Category", "Integration")]
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
        var result = JsonConvert.DeserializeObject<GetCampaignResponse[]>(content);
        
        var fetchedCampaign = result?.SingleOrDefault(c => c.CampaignName == campaign.CampaignName);
        
        fetchedCampaign.Should().NotBeNull();
        fetchedCampaign?.CampaignDescription.Should().Be(campaign.CampaignDescription);
    }

    [Fact]
    public async Task AddHero_PublishesHeroAddedEventThroughNetPub_ReturnsCreated()
    {
        // Arrange
        var campaignName = $"Messaging Campaign {Guid.NewGuid():N}";
        var createResponse = await _client.PostAsJsonAsync("/api/Campaign", new
        {
            CampaignName = campaignName,
            CampaignDescription = "Publishes HeroAddedDomainEvent to RabbitMQ",
            CampaignImage = "test-image.jpg",
            CreatedDate = DateTime.UtcNow
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var request = new
        {
            Hero = new
            {
                Name = "Bruenor",
                Class = "Barbarian",
                Race = "Human",
                Alignment = "Lawful",
                Level = 1,
                Experience = 0,
                HitPoints = 12,
                HitDice = "D12"
            }
        };

        // Act: the handler publishes HeroAddedDomainEvent through IEventPublisher -> NetPub -> RabbitMQ
        // with publisher confirms, so a 201 proves the broker accepted the message.
        var response = await _client.PostAsJsonAsync($"/api/Campaign/{campaignName}/heroes", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}