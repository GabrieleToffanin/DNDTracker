using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class CombatantStateModel
{
    public Guid Id { get; set; }
    public Guid ActiveCombatCampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CombatParticipantType Type { get; set; }
    public int Initiative { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }
    public bool HideHitPointsFromPlayers { get; set; }
    public int TurnOrder { get; set; }
    public ActiveCombatModel ActiveCombat { get; set; } = null!;
    public List<CombatantConditionModel> Conditions { get; private set; } = [];

    public static CombatantStateModel From(CombatantState combatant, int turnOrder) => new()
    {
        Id = combatant.Id,
        Name = combatant.Name,
        Type = combatant.Type,
        Initiative = combatant.Initiative,
        CurrentHitPoints = combatant.CurrentHitPoints,
        MaxHitPoints = combatant.MaxHitPoints,
        TemporaryHitPoints = combatant.TemporaryHitPoints,
        HideHitPointsFromPlayers = combatant.HideHitPointsFromPlayers,
        TurnOrder = turnOrder,
        Conditions = combatant.Conditions.Select(CombatantConditionModel.From).ToList()
    };

    public CombatantState ToValueObject() => new(
        Id,
        Name,
        Type,
        Initiative,
        CurrentHitPoints,
        MaxHitPoints,
        TemporaryHitPoints,
        HideHitPointsFromPlayers,
        Conditions.Select(condition => condition.ToValueObject()).ToList());

    public void Apply(CombatantStateModel source)
    {
        Name = source.Name;
        Type = source.Type;
        Initiative = source.Initiative;
        CurrentHitPoints = source.CurrentHitPoints;
        MaxHitPoints = source.MaxHitPoints;
        TemporaryHitPoints = source.TemporaryHitPoints;
        HideHitPointsFromPlayers = source.HideHitPointsFromPlayers;
        TurnOrder = source.TurnOrder;
        Conditions.Clear();
        Conditions.AddRange(source.Conditions);
    }
}
