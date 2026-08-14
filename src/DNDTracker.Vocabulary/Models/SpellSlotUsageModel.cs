using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class SpellSlotUsageModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public int SlotLevel { get; set; }
    public int SlotsTotal { get; set; }
    public int SlotsSpent { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static SpellSlotUsageModel From(SpellSlotUsage usage) => new()
    {
        Id = Guid.NewGuid(),
        SlotLevel = usage.SlotLevel,
        SlotsTotal = usage.SlotsTotal,
        SlotsSpent = usage.SlotsSpent
    };

    public SpellSlotUsage ToValueObject() => new(SlotLevel, SlotsTotal, SlotsSpent);

    public void Apply(SpellSlotUsageModel source)
    {
        SlotLevel = source.SlotLevel;
        SlotsTotal = source.SlotsTotal;
        SlotsSpent = source.SlotsSpent;
    }
}
