using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
namespace DNDTracker.Domain.Tests.Behaviors;

/// <summary>
/// Provides a reusable specification for testing various implementations
/// of the <see cref="Campaign.ICampaignRepository"/> interface.
/// </summary>
/// <remarks>
/// This abstract class serves as a base for creating test scenarios and behaviors
/// to validate the functionality of campaign repository implementations.
/// It utilizes a <see cref="WebApplicationFactory{TEntryPoint}"/> to simulate
/// the application environment for integration testing purposes.
/// </remarks>
/// <example>
/// Implementations of this specification should derive from this class and provide
/// their own specific <see cref="Campaign.ICampaignRepository"/> setup.
/// </example>
/// <seealso cref="WebApplicationFactory{TEntryPoint}"/>
/// <seealso cref="Campaign.ICampaignRepository"/>
/// <seealso cref="Campaign"/>
public abstract class CampaignRepositorySpecification
{
    protected internal ICampaignRepository _campaignRepository;
    
    [Fact]
    public async Task GivenCorrectData_WhenCreatingCampaign_ThenCampaignIsCreated()
    {
        Campaign campaign = Campaign.Create(
            "Test Campaigns",
            "Test Campaigns Description",
            "testurl.jpg",
            DateTime.UtcNow,
            true);
        
        await _campaignRepository.CreateCampaignAsync(campaign, CancellationToken.None);
        
        var createdCampaign = await _campaignRepository.GetCampaignAsync("Test Campaigns", CancellationToken.None);
        
        createdCampaign.Should().BeEquivalentTo(campaign);
    }

    [Fact]
    public async Task GivenTrackerState_WhenCreatingCampaign_ThenTrackerStateIsRestored()
    {
        var hero = Hero.Create(
            Guid.NewGuid(),
            "Ari",
            HeroClass.Wizard,
            Race.Human,
            Alignment.Neutral | Alignment.Good,
            3,
            900,
            12,
            DiceType.D6,
            false,
            new AbilityScores(8, 14, 12, 16, 10, 10),
            10,
            12,
            2,
            13,
            4,
            30,
            "Hero notes",
            "Sage",
            [new InventoryItem(Guid.NewGuid(), "Torch", 3, "Bright")],
            [new InventoryItem(Guid.NewGuid(), "Wand", 1)],
            [new CharacterSpellEntry(1, "Magic Missile", true)],
            [new SpellSlotUsage(1, 4, 1)],
            [new CharacterCondition("Blessed", 3)]);

        var combatantId = Guid.NewGuid();
        var campaign = Campaign.Create(
            Guid.NewGuid(),
            "Tracker Campaign",
            "A campaign with relational tracker state",
            "tracker.png",
            DateTime.UtcNow,
            true,
            [hero],
            [new MonsterStatBlock(Guid.NewGuid(), "Goblin", "Humanoid", 15, 7, 1, 200, 2, 30)],
            new CombatState(
                2,
                0,
                [new CombatantState(combatantId, "Goblin", CombatParticipantType.Monster, 14, 5, 7, 0, false,
                    [new CharacterCondition("Poisoned", 1)])]),
            [new SessionLogEntry(Guid.NewGuid(), DateTime.UtcNow, 180, "Introductions", "Keep moving")],
            [new CampaignTimelineEntry(Guid.NewGuid(), DateTime.UtcNow, "Campaign started")],
            [new NpcResource(Guid.NewGuid(), "Mira", "Guide", "Knows the road")],
            [new LocationResource(Guid.NewGuid(), "Ravenwood", "A dark forest", "https://example.test/map")],
            [new QuestResource(Guid.NewGuid(), "Find the relic", QuestStatus.Active, "Recover it")],
            [new LootResource(Guid.NewGuid(), "Moonstone", true, "Glows at night")],
            [new CampaignMember(Guid.NewGuid(), "Dungeon Master", CampaignMemberRole.DungeonMaster)]);

        await _campaignRepository.CreateCampaignAsync(campaign, CancellationToken.None);

        var restoredCampaign = await _campaignRepository.GetCampaignAsync("Tracker Campaign", CancellationToken.None);

        restoredCampaign.Should().BeEquivalentTo(campaign);

        campaign.AddCombatCondition(combatantId, new CharacterCondition("Stunned", 2));
        campaign.Heroes.Single().AddCondition(new CharacterCondition("Invisible", null));
        campaign.AddLoot(new LootResource(Guid.NewGuid(), "Healing Potion", false, "Restores health"));

        await _campaignRepository.UpdateAsync(campaign, CancellationToken.None);

        var updatedCampaign = await _campaignRepository.GetCampaignAsync("Tracker Campaign", CancellationToken.None);

        updatedCampaign.Should().BeEquivalentTo(campaign);
    }
}