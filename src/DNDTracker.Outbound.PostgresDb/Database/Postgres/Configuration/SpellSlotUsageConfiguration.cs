using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class SpellSlotUsageConfiguration : IEntityTypeConfiguration<SpellSlotUsageModel>
{
    public void Configure(EntityTypeBuilder<SpellSlotUsageModel> builder)
    {
        builder.HasKey(slot => slot.Id);
        builder.Property(slot => slot.Id).ValueGeneratedNever();
    }
}
