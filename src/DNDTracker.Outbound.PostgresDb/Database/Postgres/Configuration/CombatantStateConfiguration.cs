using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CombatantStateConfiguration : IEntityTypeConfiguration<CombatantStateModel>
{
    public void Configure(EntityTypeBuilder<CombatantStateModel> builder)
    {
        builder.HasKey(combatant => combatant.Id);
        builder.Property(combatant => combatant.Id).ValueGeneratedNever();
        builder.HasOne(combatant => combatant.ActiveCombat)
            .WithMany(combat => combat.InitiativeOrder)
            .HasForeignKey(combatant => combatant.ActiveCombatCampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(combatant => combatant.Conditions)
            .WithOne(condition => condition.Combatant)
            .HasForeignKey(condition => condition.CombatantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
