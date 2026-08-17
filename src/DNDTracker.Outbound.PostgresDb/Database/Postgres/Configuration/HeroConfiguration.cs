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

        builder.HasOne(h => h.Campaign)
            .WithMany(c => c.Heroes)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<SpellModel>(h => h.Spells)
            .WithMany(s => s.Heroes);

        builder.HasMany(h => h.Inventory)
            .WithOne(item => item.Hero)
            .HasForeignKey(item => item.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Equipment)
            .WithOne(item => item.Hero)
            .HasForeignKey(item => item.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Spellbook)
            .WithOne(entry => entry.Hero)
            .HasForeignKey(entry => entry.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.SpellSlots)
            .WithOne(slot => slot.Hero)
            .HasForeignKey(slot => slot.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Conditions)
            .WithOne(condition => condition.Hero)
            .HasForeignKey(condition => condition.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.SavingThrowProficiencies)
            .WithOne(p => p.Hero)
            .HasForeignKey(p => p.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.SkillProficiencies)
            .WithOne(p => p.Hero)
            .HasForeignKey(p => p.HeroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Feats)
            .WithOne(f => f.Hero)
            .HasForeignKey(f => f.HeroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
