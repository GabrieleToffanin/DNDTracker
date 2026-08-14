using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class ActiveCombatModel
{
    public Guid CampaignId { get; set; }
    public int Round { get; set; }
    public int TurnIndex { get; set; }
    public CampaignModel Campaign { get; set; } = null!;
    public List<CombatantStateModel> InitiativeOrder { get; private set; } = [];

    public static ActiveCombatModel From(CombatState combat) => new()
    {
        Round = combat.Round,
        TurnIndex = combat.TurnIndex,
        InitiativeOrder = combat.InitiativeOrder
            .Select((combatant, index) => CombatantStateModel.From(combatant, index))
            .ToList()
    };

    public CombatState ToValueObject() => new(
        Round,
        TurnIndex,
        InitiativeOrder
            .OrderBy(combatant => combatant.TurnOrder)
            .Select(combatant => combatant.ToValueObject())
            .ToList());

    public void Apply(ActiveCombatModel source)
    {
        Round = source.Round;
        TurnIndex = source.TurnIndex;

        var existingById = InitiativeOrder.ToDictionary(combatant => combatant.Id);
        var sourceIds = new HashSet<Guid>();

        foreach (var combatant in source.InitiativeOrder)
        {
            sourceIds.Add(combatant.Id);

            if (existingById.TryGetValue(combatant.Id, out var existing))
                existing.Apply(combatant);
            else
                InitiativeOrder.Add(combatant);
        }

        InitiativeOrder.RemoveAll(combatant => !sourceIds.Contains(combatant.Id));
    }
}
