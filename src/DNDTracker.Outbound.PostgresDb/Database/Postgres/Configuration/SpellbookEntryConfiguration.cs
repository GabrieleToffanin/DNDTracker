using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class SpellbookEntryConfiguration : IEntityTypeConfiguration<SpellbookEntryModel>
{
    public void Configure(EntityTypeBuilder<SpellbookEntryModel> builder)
    {
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).ValueGeneratedNever();
    }
}
