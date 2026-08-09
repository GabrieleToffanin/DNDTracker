using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbounx.PostgresDb.Database.Postgres.Configuration;

public class HeroConfiguration : IEntityTypeConfiguration<HeroModel>
{
    public void Configure(EntityTypeBuilder<HeroModel> builder)
    {
        builder.HasKey(c => c.Id);

        // The Id is generated in the domain layer (HeroId), not by the database,
        // so EF Core must not try to generate/override it on insert.
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.HasOne(h => h.Campaign)
            .WithMany(c => c.Heroes);

        builder.HasMany<SpellModel>(h => h.Spells)
            .WithMany(s => s.Heroes);
    }
}