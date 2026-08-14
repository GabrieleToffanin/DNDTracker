using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class SessionLogEntryConfiguration : IEntityTypeConfiguration<SessionLogEntryModel>
{
    public void Configure(EntityTypeBuilder<SessionLogEntryModel> builder)
    {
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).ValueGeneratedNever();
        builder.HasOne(entry => entry.Campaign)
            .WithMany(campaign => campaign.SessionLogs)
            .HasForeignKey(entry => entry.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
