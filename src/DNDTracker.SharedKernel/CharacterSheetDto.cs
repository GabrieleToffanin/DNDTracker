using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.SharedKernel;

public sealed record CharacterSheetDto(
    Guid Id,
    string Name,
    HeroClass Class,
    Race Race,
    Alignment Alignment,
    int Level,
    int Experience,
    bool IsNonPlayerCharacter,
    AbilityScores AbilityScores,
    int CurrentHitPoints,
    int MaxHitPoints,
    int TemporaryHitPoints,
    int ArmorClass,
    int Initiative,
    int Speed,
    DiceType HitDice,
    IReadOnlyCollection<InventoryItem> Inventory,
    IReadOnlyCollection<InventoryItem> Equipment,
    IReadOnlyCollection<CharacterSpellEntry> Spellbook,
    IReadOnlyCollection<SpellSlotUsage> SpellSlots,
    IReadOnlyCollection<CharacterCondition> Conditions,
    string Notes,
    string Background);
