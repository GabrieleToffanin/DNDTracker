using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class HeroConditionConfiguration : IEntityTypeConfiguration<HeroConditionModel>
{
    public void Configure(EntityTypeBuilder<HeroConditionModel> builder)
    {
        builder.HasKey(condition => condition.Id);
        builder.Property(condition => condition.Id).ValueGeneratedNever();
    }
}
