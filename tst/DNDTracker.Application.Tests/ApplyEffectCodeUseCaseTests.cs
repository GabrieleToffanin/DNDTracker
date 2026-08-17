using DNDTracker.Application.Tests.Behaviors.Dummies;
using DNDTracker.Application.UseCases.Campaigns.Tracker;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FluentAssertions;
using Force.DeepCloner;
using Xunit;

namespace DNDTracker.Application.Tests;

public class ApplyEffectCodeUseCaseTests
{
    private readonly DummyCampaignRepository _repository;
    private readonly ApplyEffectCodeCommandHandler _handler;
    private readonly Campaign _campaign;
    private readonly Hero _hero;

    public ApplyEffectCodeUseCaseTests()
    {
        _repository = new DummyCampaignRepository();
        _handler = new ApplyEffectCodeCommandHandler(_repository);

        _hero = Hero.Create("Aria", HeroClass.Bard, Race.HalfElf, Alignment.Good, 5, 0, 30, DiceType.D8);
        _campaign = Campaign.Create("TestCampaign", "desc", "https://example.com/img.jpg", DateTime.UtcNow, true);
        _campaign.AddHero(_hero);
        _repository.Insert(_campaign.DeepClone());
    }

    [Fact]
    public async Task GivenValidEffectCode_WhenApplied_ThenConditionIsAddedToHero()
    {
        var command = new ApplyEffectCodeCommand("TestCampaign", _hero.Id.Id, "ATK:2; ADVATK", DurationRounds: 3);

        await _handler.Handle(command, CancellationToken.None);

        var stored = _repository.Campaigns["TestCampaign"];
        var hero = stored.Heroes.Single(h => h.Id.Id == _hero.Id.Id);
        hero.Conditions.Should().HaveCount(1);
        hero.Conditions[0].EffectCode.Should().NotBeNull();
        hero.Conditions[0].RemainingRounds.Should().Be(3);
        hero.Conditions[0].EffectCode!.ParsedTokens.Should().HaveCount(2);
    }

    [Fact]
    public async Task GivenUnknownHero_WhenApplied_ThenThrows()
    {
        var command = new ApplyEffectCodeCommand("TestCampaign", Guid.NewGuid(), "ATK:1");

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();
    }
}
