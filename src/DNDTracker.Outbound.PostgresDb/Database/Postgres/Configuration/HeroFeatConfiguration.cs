using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class HeroFeatConfiguration : IEntityTypeConfiguration<HeroFeatModel>
{
    public void Configure(EntityTypeBuilder<HeroFeatModel> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();
        builder.Property(f => f.FeatName).IsRequired().HasMaxLength(200);
    }
}
