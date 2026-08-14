namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddCharacterConditionRequest(
    string Condition,
    int? RemainingRounds);
