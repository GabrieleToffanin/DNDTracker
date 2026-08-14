using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddCharacterConditionCommand(string CampaignName, Guid CharacterId, CharacterCondition Condition) : ICommand;
