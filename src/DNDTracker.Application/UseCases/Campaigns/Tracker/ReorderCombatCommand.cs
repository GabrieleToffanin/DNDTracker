using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record ReorderCombatCommand(string CampaignName, Guid CombatantId, int TargetIndex) : ICommand;
