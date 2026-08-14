using DNDTracker.DataAccessObject.Mapping.Json;
using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.ValueObjects;
using DNDTracker.Vocabulary.Models;

namespace DNDTracker.DataAccessObject.Mapping.HeroMap;

public static class HeroModelMapping
{
    public static Hero MapToDomain(this HeroModel heroModel)
    {
        return Hero.Create(
            heroModel.Id,
            heroModel.Name,
            heroModel.Class,
            heroModel.Race,
            heroModel.Alignment,
            heroModel.Level,
            heroModel.Experience,
            heroModel.HitPoints,
            heroModel.HitDice,
            heroModel.IsNonPlayerCharacter,
            new AbilityScores(
                heroModel.Strength,
                heroModel.Dexterity,
                heroModel.Constitution,
                heroModel.Intelligence,
                heroModel.Wisdom,
                heroModel.Charisma),
            heroModel.CurrentHitPoints,
            heroModel.MaxHitPoints,
            heroModel.TemporaryHitPoints,
            heroModel.ArmorClass,
            heroModel.Initiative,
            heroModel.Speed,
            heroModel.Notes,
            heroModel.Background,
            JsonCollectionMapper.DeserializeCollection<InventoryItem>(heroModel.InventoryJson),
            JsonCollectionMapper.DeserializeCollection<InventoryItem>(heroModel.EquipmentJson),
            JsonCollectionMapper.DeserializeCollection<CharacterSpellEntry>(heroModel.SpellbookJson),
            JsonCollectionMapper.DeserializeCollection<SpellSlotUsage>(heroModel.SpellSlotsJson),
            JsonCollectionMapper.DeserializeCollection<CharacterCondition>(heroModel.ConditionsJson)
        );
    }

    public static HeroModel MapToModel(this Hero hero)
    {
        return new HeroModel()
        {
            Id = hero.Id.Id,
            Name = hero.Name,
            Class = hero.Class,
            Race = hero.Race,
            Alignment = hero.Alignment,
            Level = hero.Level,
            Experience = hero.Experience,
            HitPoints = hero.HitPoints,
            HitDice = hero.HitDice,
            IsNonPlayerCharacter = hero.IsNonPlayerCharacter,
            Strength = hero.AbilityScores.Strength,
            Dexterity = hero.AbilityScores.Dexterity,
            Constitution = hero.AbilityScores.Constitution,
            Intelligence = hero.AbilityScores.Intelligence,
            Wisdom = hero.AbilityScores.Wisdom,
            Charisma = hero.AbilityScores.Charisma,
            CurrentHitPoints = hero.CurrentHitPoints,
            MaxHitPoints = hero.MaxHitPoints,
            TemporaryHitPoints = hero.TemporaryHitPoints,
            ArmorClass = hero.ArmorClass,
            Initiative = hero.Initiative,
            Speed = hero.Speed,
            Notes = hero.Notes,
            Background = hero.Background,
            InventoryJson = JsonCollectionMapper.Serialize(hero.Inventory),
            EquipmentJson = JsonCollectionMapper.Serialize(hero.Equipment),
            SpellbookJson = JsonCollectionMapper.Serialize(hero.Spellbook),
            SpellSlotsJson = JsonCollectionMapper.Serialize(hero.SpellSlots),
            ConditionsJson = JsonCollectionMapper.Serialize(hero.Conditions)
        };
    }
}
