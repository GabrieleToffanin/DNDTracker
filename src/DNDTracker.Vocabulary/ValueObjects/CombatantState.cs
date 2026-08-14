using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CombatantState(
    Guid Id,
    string Name,
    CombatParticipantType Type,
    int Initiative,
    int CurrentHitPoints,
    int MaxHitPoints,
    int TemporaryHitPoints,
    bool HideHitPointsFromPlayers,
    IReadOnlyCollection<CharacterCondition> Conditions);
