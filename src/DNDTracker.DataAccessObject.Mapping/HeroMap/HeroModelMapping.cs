using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using DNDTracker.Vocabulary.Models;

namespace DNDTracker.DataAccessObject.Mapping.HeroMap;

public static class HeroModelMapping
{
    public static Hero MapToDomain(this HeroModel heroModel)
    {
        var savingThrowProficiencies = heroModel.SavingThrowProficiencies.Select(p => p.Ability).ToList();
        var skillProficiencies = heroModel.SkillProficiencies.Select(p => p.Skill).ToList();
        var feats = heroModel.Feats.Select(f => f.FeatName).ToList();

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
            heroModel.Inventory.Select(item => item.ToValueObject()),
            heroModel.Equipment.Select(item => item.ToValueObject()),
            heroModel.Spellbook.Select(entry => entry.ToValueObject()),
            heroModel.SpellSlots.Select(slot => slot.ToValueObject()),
            heroModel.Conditions.Select(condition => condition.ToValueObject()),
            new DeathSaves(heroModel.DeathSaveSuccesses, heroModel.DeathSaveFailures),
            new SavingThrowProficiencies(savingThrowProficiencies),
            new SkillProficiencies(skillProficiencies),
            new CharacterPersonality(
                heroModel.PersonalityTraits,
                heroModel.Ideals,
                heroModel.Bonds,
                heroModel.Flaws),
            feats,
            heroModel.SpellcastingAbility,
            heroModel.Spells.Select(MapToValueObject)
        );
    }

    public static HeroModel MapToModel(this Hero hero)
    {
        var heroId = hero.Id.Id;

        var heroModel = new HeroModel
        {
            Id = heroId,
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
            DeathSaveSuccesses = hero.DeathSaves.Successes,
            DeathSaveFailures = hero.DeathSaves.Failures,
            PersonalityTraits = hero.Personality.PersonalityTraits,
            Ideals = hero.Personality.Ideals,
            Bonds = hero.Personality.Bonds,
            Flaws = hero.Personality.Flaws,
            SpellcastingAbility = hero.SpellcastingAbility
        };

        heroModel.SavingThrowProficiencies.AddRange(
            hero.SavingThrowProficiencies.Proficient.Select(a => HeroSavingThrowProficiencyModel.From(heroId, a)));
        heroModel.SkillProficiencies.AddRange(
            hero.SkillProficiencies.Proficient.Select(s => HeroSkillProficiencyModel.From(heroId, s)));
        heroModel.Feats.AddRange(
            hero.Feats.Select(f => HeroFeatModel.From(heroId, f)));
        heroModel.Inventory.AddRange(hero.Inventory.Select(InventoryItemModel.From));
        heroModel.Equipment.AddRange(hero.Equipment.Select(EquipmentItemModel.From));
        heroModel.Spells.UnionWith(hero.Spells.Select(MapToModel));
        heroModel.Spellbook.AddRange(hero.Spellbook.Select(SpellbookEntryModel.From));
        heroModel.SpellSlots.AddRange(hero.SpellSlots.Select(SpellSlotUsageModel.From));
        heroModel.Conditions.AddRange(hero.Conditions.Select(HeroConditionModel.From));

        return heroModel;
    }

    private static Spell MapToValueObject(SpellModel spellModel) => new()
    {
        Id = spellModel.Id,
        Name = spellModel.Name,
        Description = spellModel.Description,
        Source = spellModel.Source,
        Level = spellModel.Level,
        School = spellModel.School,
        Time = spellModel.Time,
        Range = spellModel.Range,
        Components = spellModel.Components,
        Material = spellModel.Material,
        IsRitual = spellModel.IsRitual,
        Duration = spellModel.Duration,
        Concentration = spellModel.Concentration,
        CastingTime = spellModel.CastingTime,
        Damage = spellModel.Damage,
        Save = spellModel.Save,
        EffectCode = spellModel.EffectCode is null ? null : new EffectCode(spellModel.EffectCode)
    };

    private static SpellModel MapToModel(Spell spell) => new()
    {
        Id = spell.Id,
        Name = spell.Name,
        Description = spell.Description,
        Source = spell.Source,
        Level = spell.Level,
        School = spell.School,
        Time = spell.Time,
        Range = spell.Range,
        Components = spell.Components,
        Material = spell.Material,
        IsRitual = spell.IsRitual,
        Duration = spell.Duration,
        Concentration = spell.Concentration,
        CastingTime = spell.CastingTime,
        Damage = spell.Damage,
        Save = spell.Save,
        EffectCode = spell.EffectCode?.Raw
    };
}
