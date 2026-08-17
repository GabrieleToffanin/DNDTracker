using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class HeroSavingThrowProficiencyConfiguration : IEntityTypeConfiguration<HeroSavingThrowProficiencyModel>
{
    public void Configure(EntityTypeBuilder<HeroSavingThrowProficiencyModel> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
    }
}
