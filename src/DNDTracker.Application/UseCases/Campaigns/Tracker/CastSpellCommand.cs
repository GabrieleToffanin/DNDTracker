using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record CastSpellCommand(
    string CampaignName,
    Guid CasterHeroId,
    Guid TargetHeroId,
    int SpellId,
    int SlotLevel,
    int? DiceRoll = null) : ICommand;
