using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class LocationResourceConfiguration : IEntityTypeConfiguration<LocationResourceModel>
{
    public void Configure(EntityTypeBuilder<LocationResourceModel> builder)
    {
        builder.HasKey(location => location.Id);
        builder.Property(location => location.Id).ValueGeneratedNever();
        builder.HasOne(location => location.Campaign)
            .WithMany(campaign => campaign.Locations)
            .HasForeignKey(location => location.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
