using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AdvanceCombatTurnCommand(string CampaignName) : ICommand;
