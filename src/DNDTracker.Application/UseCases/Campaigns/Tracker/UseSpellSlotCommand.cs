using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record UseSpellSlotCommand(
    string CampaignName,
    Guid HeroId,
    int SlotLevel) : ICommand;
