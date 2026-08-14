using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record StartCombatCommand(string CampaignName, IReadOnlyCollection<CombatantState> Combatants) : ICommand;
