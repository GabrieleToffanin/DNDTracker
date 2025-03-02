using DNDTracker.Domain.Heroes;
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
            heroModel.HitDice
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
            HitDice = hero.HitDice
        };
    }
}