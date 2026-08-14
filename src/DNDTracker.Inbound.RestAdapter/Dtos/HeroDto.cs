using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Inbound.RestAdapter.Dtos;

public record HeroDto(
    string Name,
    HeroClass Class,
    Race Race,
    Alignment Alignment,
    int Level,
    int Experience,
    int HitPoints,
    DiceType HitDice,
    bool IsNonPlayerCharacter = false,
    AbilityScores? AbilityScores = null,
    int? CurrentHitPoints = null,
    int? MaxHitPoints = null,
    int TemporaryHitPoints = 0,
    int ArmorClass = 10,
    int Initiative = 0,
    int Speed = 30,
    string Notes = "",
    string Background = "",
    IReadOnlyCollection<InventoryItem>? Inventory = null,
    IReadOnlyCollection<InventoryItem>? Equipment = null,
    IReadOnlyCollection<CharacterSpellEntry>? Spellbook = null,
    IReadOnlyCollection<SpellSlotUsage>? SpellSlots = null,
    IReadOnlyCollection<CharacterCondition>? Conditions = null);
