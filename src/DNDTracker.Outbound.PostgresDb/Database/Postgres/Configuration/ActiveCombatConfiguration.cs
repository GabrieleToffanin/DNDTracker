using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class ActiveCombatConfiguration : IEntityTypeConfiguration<ActiveCombatModel>
{
    public void Configure(EntityTypeBuilder<ActiveCombatModel> builder)
    {
        builder.HasKey(combat => combat.CampaignId);
        builder.Property(combat => combat.CampaignId).ValueGeneratedNever();
        builder.HasOne(combat => combat.Campaign)
            .WithOne(campaign => campaign.ActiveCombat)
            .HasForeignKey<ActiveCombatModel>(combat => combat.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
