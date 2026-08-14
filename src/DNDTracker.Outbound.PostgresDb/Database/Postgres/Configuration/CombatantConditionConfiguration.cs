using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CombatantConditionConfiguration : IEntityTypeConfiguration<CombatantConditionModel>
{
    public void Configure(EntityTypeBuilder<CombatantConditionModel> builder)
    {
        builder.HasKey(condition => condition.Id);
        builder.Property(condition => condition.Id).ValueGeneratedNever();
    }
}
