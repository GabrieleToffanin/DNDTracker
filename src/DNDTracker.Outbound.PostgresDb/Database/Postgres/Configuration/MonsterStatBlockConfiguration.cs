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
        builder.Property(monster => monster.Description).HasColumnType("text");
        builder.Property(monster => monster.Statistics).HasColumnType("text");
        builder.Property(monster => monster.Actions).HasColumnType("text");
        builder.Property(monster => monster.BonusActions).HasColumnType("text");
        builder.Property(monster => monster.Reactions).HasColumnType("text");
        builder.Property(monster => monster.LegendaryActions).HasColumnType("text");
        builder.Property(monster => monster.LairActions).HasColumnType("text");
        builder.Property(monster => monster.Spells).HasColumnType("text");
        builder.HasOne(monster => monster.Campaign)
            .WithMany(campaign => campaign.MonsterLibrary)
            .HasForeignKey(monster => monster.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
