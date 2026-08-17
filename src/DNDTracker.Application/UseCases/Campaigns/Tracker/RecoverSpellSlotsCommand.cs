using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record RecoverSpellSlotsCommand(
    string CampaignName,
    Guid HeroId,
    RestType RestType) : ICommand;
