using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class SpellbookEntryModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public int SpellId { get; set; }
    public string SpellName { get; set; } = string.Empty;
    public bool IsPrepared { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static SpellbookEntryModel From(CharacterSpellEntry entry) => new()
    {
        Id = Guid.NewGuid(),
        SpellId = entry.SpellId,
        SpellName = entry.SpellName,
        IsPrepared = entry.IsPrepared
    };

    public CharacterSpellEntry ToValueObject() => new(SpellId, SpellName, IsPrepared);

    public void Apply(SpellbookEntryModel source)
    {
        SpellId = source.SpellId;
        SpellName = source.SpellName;
        IsPrepared = source.IsPrepared;
    }
}
