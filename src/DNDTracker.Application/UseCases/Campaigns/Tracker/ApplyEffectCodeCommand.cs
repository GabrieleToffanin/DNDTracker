using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record ApplyEffectCodeCommand(
    string CampaignName,
    Guid HeroId,
    string RawEffectCode,
    int? DurationRounds = null) : ICommand;
