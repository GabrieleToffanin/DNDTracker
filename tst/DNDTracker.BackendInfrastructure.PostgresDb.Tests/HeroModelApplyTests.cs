using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.Models;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Tests;

public class HeroModelApplyTests
{
    [Fact]
    public void GivenUnchangedHeroCollections_WhenApplyingUpdate_ThenExistingSimpleRelationIdsArePreserved()
    {
        var existingSavingThrowId = Guid.NewGuid();
        var existingSkillId = Guid.NewGuid();
        var existingFeatId = Guid.NewGuid();
        var heroId = Guid.NewGuid();

        var current = new HeroModel
        {
            Id = heroId,
            Name = "Ari"
        };
        current.SavingThrowProficiencies.Add(new HeroSavingThrowProficiencyModel
        {
            Id = existingSavingThrowId,
            HeroId = heroId,
            Ability = AbilityType.Wisdom
        });
        current.SkillProficiencies.Add(new HeroSkillProficiencyModel
        {
            Id = existingSkillId,
            HeroId = heroId,
            Skill = SkillType.Arcana
        });
        current.Feats.Add(new HeroFeatModel
        {
            Id = existingFeatId,
            HeroId = heroId,
            FeatName = "War Caster"
        });

        var update = new HeroModel
        {
            Id = heroId,
            Name = "Ari"
        };
        update.SavingThrowProficiencies.Add(HeroSavingThrowProficiencyModel.From(heroId, AbilityType.Wisdom));
        update.SkillProficiencies.Add(HeroSkillProficiencyModel.From(heroId, SkillType.Arcana));
        update.Feats.Add(HeroFeatModel.From(heroId, "War Caster"));

        current.Apply(update);

        Assert.Collection(current.SavingThrowProficiencies, item => Assert.Equal(existingSavingThrowId, item.Id));
        Assert.Collection(current.SkillProficiencies, item => Assert.Equal(existingSkillId, item.Id));
        Assert.Collection(current.Feats, item => Assert.Equal(existingFeatId, item.Id));
    }
}
