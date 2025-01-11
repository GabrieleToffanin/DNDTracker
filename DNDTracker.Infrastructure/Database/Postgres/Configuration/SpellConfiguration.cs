using DNDTracker.Vocabulary.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Infrastructure.Database.Postgres.Configuration;

public class SpellConfiguration : IEntityTypeConfiguration<Spell>
{
    public void Configure(EntityTypeBuilder<Spell> builder)
    {
        builder.HasKey(x => x.Id);
    }
}