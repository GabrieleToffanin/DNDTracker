using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class SpellConfiguration : IEntityTypeConfiguration<SpellModel>
{
    public void Configure(EntityTypeBuilder<SpellModel> builder)
    {
        builder.HasKey(x => x.Id);
    }
}