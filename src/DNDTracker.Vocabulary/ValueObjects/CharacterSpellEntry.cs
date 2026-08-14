namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CharacterSpellEntry(
    int SpellId,
    string SpellName,
    bool IsPrepared);
