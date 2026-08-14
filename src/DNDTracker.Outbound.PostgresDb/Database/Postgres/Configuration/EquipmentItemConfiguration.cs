using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class EquipmentItemConfiguration : IEntityTypeConfiguration<EquipmentItemModel>
{
    public void Configure(EntityTypeBuilder<EquipmentItemModel> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedNever();
    }
}
