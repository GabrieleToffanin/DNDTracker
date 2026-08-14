using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class NpcResourceConfiguration : IEntityTypeConfiguration<NpcResourceModel>
{
    public void Configure(EntityTypeBuilder<NpcResourceModel> builder)
    {
        builder.HasKey(npc => npc.Id);
        builder.Property(npc => npc.Id).ValueGeneratedNever();
        builder.HasOne(npc => npc.Campaign)
            .WithMany(campaign => campaign.Npcs)
            .HasForeignKey(npc => npc.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
