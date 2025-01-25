using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Application.UseCases.Campaigns.AddHero;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;
using Force.DeepCloner;
using Xunit;

namespace DNDTracker.Application.Tests;

public class AddHeroToCampaignUseCaseTests
{
    private readonly DummyCampaignRepository _campaignRepository;
    private readonly AddHeroToCampaignCommandHandler _handler;
    private readonly Campaign _expectedCampaign;

    public AddHeroToCampaignUseCaseTests()
    {
        _campaignRepository = new DummyCampaignRepository();
        _handler = new AddHeroToCampaignCommandHandler(_campaignRepository);
        _expectedCampaign = SetupExpectedCampaign(DateTime.Now);
        _campaignRepository.Insert(_expectedCampaign.DeepClone());
    }
    
    [Fact]
    public async Task GivenValidRequestHavingHeroes_WhenHandle_ThenHeroIsAddedToCampaign()
    {
        // Given
        var hero = CreateHero();
        var command = CreateAddHeroToCampaignCommand(hero);

        //When
        await _handler.Handle(command, CancellationToken.None);
        
        //Then
        _campaignRepository.AssertCampaignEquals(_expectedCampaign
            .WithHeroes(hero));
    }

    private static Campaign SetupExpectedCampaign(DateTime createDate)
    {
        var expectedCampaign = Campaign.Create(
            "Test Campaigns",
            "Test Campaigns Description",
            "testurl.jpg",
            createDate,
            true);
        
        return expectedCampaign;
    }

    private static AddHeroToCampaignCommand CreateAddHeroToCampaignCommand(Hero hero)
    {
        AddHeroToCampaignCommand command = new AddHeroToCampaignCommand(
            "Test Campaigns",
            hero);
        return command;
    }

    private static Hero CreateHero()
    {
        Hero hero = Hero.Create(
            "Ludwin",
            HeroClass.Paladin,
            Race.HalfElf,
            Alignment.Good,
            1,
            0,
            10,
            DiceType.D4);
        return hero;
    }
}