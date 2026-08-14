using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record StartCombatRequest(
    IReadOnlyCollection<CombatantInput> Combatants);

public sealed record CombatantInput(
    Guid Id,
    string Name,
    CombatParticipantType Type,
    int Initiative,
    int CurrentHitPoints,
    int MaxHitPoints,
    int TemporaryHitPoints,
    bool HideHitPointsFromPlayers,
    IReadOnlyCollection<CharacterCondition>? Conditions);
