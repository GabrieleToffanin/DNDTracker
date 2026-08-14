using DNDTracker.SharedKernel.Queries;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.Queries.UseCases.RollDice;

public sealed record RollDiceInCampaign(
    string CampaignName,
    string Expression,
    int Modifier,
    string? Context) : IQuery<DiceRollResult>;
