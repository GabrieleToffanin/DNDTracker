using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Queries;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.Queries.UseCases.RollDice;

public sealed class RollDiceInCampaignHandler(ICampaignRepository campaignRepository)
    : IQueryHandler<RollDiceInCampaign, DiceRollResult>
{
    public async Task<DiceRollResult> Handle(RollDiceInCampaign request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        ParseExpression(request.Expression, out var numberOfDice, out var diceSides);

        if (numberOfDice <= 0 || diceSides <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.Expression), "Dice expression is not valid.");

        var rolls = new List<int>(numberOfDice);
        for (var i = 0; i < numberOfDice; i++)
            rolls.Add(Random.Shared.Next(1, diceSides + 1));

        var total = rolls.Sum() + request.Modifier;

        return new DiceRollResult(request.Expression, total, rolls, request.Modifier, request.Context);
    }

    private static void ParseExpression(string expression, out int numberOfDice, out int diceSides)
    {
        var cleanedExpression = expression.Trim().ToLowerInvariant();
        var separators = cleanedExpression.Split('d', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (separators.Length != 2 || !int.TryParse(separators[0], out numberOfDice) || !int.TryParse(separators[1], out diceSides))
            throw new ArgumentException("Dice expression must be in NdM format (e.g. 1d20).", nameof(expression));
    }
}
