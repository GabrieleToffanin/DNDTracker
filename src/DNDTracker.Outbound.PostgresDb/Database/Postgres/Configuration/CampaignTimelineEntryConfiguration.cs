using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CampaignTimelineEntryConfiguration : IEntityTypeConfiguration<CampaignTimelineEntryModel>
{
    public void Configure(EntityTypeBuilder<CampaignTimelineEntryModel> builder)
    {
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).ValueGeneratedNever();
        builder.HasOne(entry => entry.Campaign)
            .WithMany(campaign => campaign.TimelineEntries)
            .HasForeignKey(entry => entry.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
