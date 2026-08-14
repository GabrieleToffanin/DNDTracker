using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CampaignMemberConfiguration : IEntityTypeConfiguration<CampaignMemberModel>
{
    public void Configure(EntityTypeBuilder<CampaignMemberModel> builder)
    {
        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.HasOne(member => member.Campaign)
            .WithMany(campaign => campaign.Members)
            .HasForeignKey(member => member.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
