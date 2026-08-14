using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record UpdateCombatantHitPointsCommand(string CampaignName, Guid CombatantId, int Damage, int Healing, int TemporaryHitPointsDelta) : ICommand;
