using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class MonsterStatBlockConfiguration : IEntityTypeConfiguration<MonsterStatBlockModel>
{
    public void Configure(EntityTypeBuilder<MonsterStatBlockModel> builder)
    {
        builder.HasKey(monster => monster.Id);
        builder.Property(monster => monster.Id).ValueGeneratedNever();
        builder.HasOne(monster => monster.Campaign)
            .WithMany(campaign => campaign.MonsterLibrary)
            .HasForeignKey(monster => monster.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
