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
    IReadOnlyCollection<CharacterCondition>? Conditions = null,
    int DeathSaveSuccesses = 0,
    int DeathSaveFailures = 0,
    IReadOnlyCollection<AbilityType>? SavingThrowProficiencies = null,
    IReadOnlyCollection<SkillType>? SkillProficiencies = null,
    string PersonalityTraits = "",
    string Ideals = "",
    string Bonds = "",
    string Flaws = "",
    IReadOnlyCollection<string>? Feats = null,
    AbilityType? SpellcastingAbility = null);
