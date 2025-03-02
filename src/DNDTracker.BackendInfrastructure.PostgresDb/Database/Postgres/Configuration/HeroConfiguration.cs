using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres.Configuration;

public class HeroConfiguration : IEntityTypeConfiguration<HeroModel>
{
    public void Configure(EntityTypeBuilder<HeroModel> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.HasOne(h => h.Campaign)
            .WithMany(c => c.Heroes);
        
        builder.HasMany<SpellModel>(h => h.Spells)
            .WithMany(s => s.Heroes);
    }
}