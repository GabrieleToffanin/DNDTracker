using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbounx.PostgresDb.Database.Postgres.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<CampaignModel>
{
    public void Configure(EntityTypeBuilder<CampaignModel> builder)
    {
        builder.HasKey(c => c.Id);;
    }
}