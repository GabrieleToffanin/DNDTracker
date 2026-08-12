using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<CampaignModel>
{
    public void Configure(EntityTypeBuilder<CampaignModel> builder)
    {
        builder.HasKey(c => c.Id);

        // The Id is generated in the domain layer (CampaignId), not by the database,
        // so EF Core must not try to generate/override it on insert.
        builder.Property(c => c.Id)
            .ValueGeneratedNever();
    }
}