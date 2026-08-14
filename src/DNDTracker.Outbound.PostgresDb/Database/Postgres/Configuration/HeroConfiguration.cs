using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class HeroConfiguration : IEntityTypeConfiguration<HeroModel>
{
    public void Configure(EntityTypeBuilder<HeroModel> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(h => h.InventoryJson).HasDefaultValue("[]");
        builder.Property(h => h.EquipmentJson).HasDefaultValue("[]");
        builder.Property(h => h.SpellbookJson).HasDefaultValue("[]");
        builder.Property(h => h.SpellSlotsJson).HasDefaultValue("[]");
        builder.Property(h => h.ConditionsJson).HasDefaultValue("[]");

        builder.HasOne(h => h.Campaign)
            .WithMany(c => c.Heroes);

        builder.HasMany<SpellModel>(h => h.Spells)
            .WithMany(s => s.Heroes);
    }
}
