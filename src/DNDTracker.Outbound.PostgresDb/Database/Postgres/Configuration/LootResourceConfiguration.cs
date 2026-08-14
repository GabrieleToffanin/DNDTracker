using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class LootResourceConfiguration : IEntityTypeConfiguration<LootResourceModel>
{
    public void Configure(EntityTypeBuilder<LootResourceModel> builder)
    {
        builder.HasKey(loot => loot.Id);
        builder.Property(loot => loot.Id).ValueGeneratedNever();
        builder.HasOne(loot => loot.Campaign)
            .WithMany(campaign => campaign.Loot)
            .HasForeignKey(loot => loot.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
