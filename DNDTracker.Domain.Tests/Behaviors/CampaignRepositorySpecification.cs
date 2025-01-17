using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
namespace DNDTracker.Domain.Tests.Behaviors;

/// <summary>
/// Provides a reusable specification for testing various implementations
/// of the <see cref="ICampaignRepository"/> interface.
/// </summary>
/// <remarks>
/// This abstract class serves as a base for creating test scenarios and behaviors
/// to validate the functionality of campaign repository implementations.
/// It utilizes a <see cref="WebApplicationFactory{TEntryPoint}"/> to simulate
/// the application environment for integration testing purposes.
/// </remarks>
/// <example>
/// Implementations of this specification should derive from this class and provide
/// their own specific <see cref="ICampaignRepository"/> setup.
/// </example>
/// <seealso cref="WebApplicationFactory{TEntryPoint}"/>
/// <seealso cref="ICampaignRepository"/>
/// <seealso cref="Campaign"/>
public abstract class CampaignRepositorySpecification
{
    protected internal ICampaignRepository _campaignRepository;
    
    [Fact]
    public async Task GivenCorrectData_WhenCreatingCampaign_ThenCampaignIsCreated()
    {
        Campaign campaign = Campaign.Create(
            "Test Campaign",
            "Test Campaign Description",
            "testurl.jpg",
            DateTime.UtcNow,
            true);
        
        await _campaignRepository.CreateCampaignAsync(campaign, CancellationToken.None);
        
        var createdCampaign = await _campaignRepository.GetCampaignAsync("Test Campaign", CancellationToken.None);
        
        createdCampaign.Should().BeEquivalentTo(campaign);
    }
}