using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class HeroSkillProficiencyConfiguration : IEntityTypeConfiguration<HeroSkillProficiencyModel>
{
    public void Configure(EntityTypeBuilder<HeroSkillProficiencyModel> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
    }
}
