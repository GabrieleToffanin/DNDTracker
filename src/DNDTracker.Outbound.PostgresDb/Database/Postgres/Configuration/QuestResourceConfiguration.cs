using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class QuestResourceConfiguration : IEntityTypeConfiguration<QuestResourceModel>
{
    public void Configure(EntityTypeBuilder<QuestResourceModel> builder)
    {
        builder.HasKey(quest => quest.Id);
        builder.Property(quest => quest.Id).ValueGeneratedNever();
        builder.HasOne(quest => quest.Campaign)
            .WithMany(campaign => campaign.Quests)
            .HasForeignKey(quest => quest.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
