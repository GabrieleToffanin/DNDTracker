using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record UpdateCharacterHitPointsCommand(string CampaignName, Guid CharacterId, int Damage, int Healing, int TemporaryHitPointsDelta) : ICommand;
