using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<CampaignModel>
{
    public void Configure(EntityTypeBuilder<CampaignModel> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.MonsterLibraryJson).HasDefaultValue("[]");
        builder.Property(c => c.SessionLogsJson).HasDefaultValue("[]");
        builder.Property(c => c.TimelineEntriesJson).HasDefaultValue("[]");
        builder.Property(c => c.NpcsJson).HasDefaultValue("[]");
        builder.Property(c => c.LocationsJson).HasDefaultValue("[]");
        builder.Property(c => c.QuestsJson).HasDefaultValue("[]");
        builder.Property(c => c.LootJson).HasDefaultValue("[]");
        builder.Property(c => c.MembersJson).HasDefaultValue("[]");
    }
}
