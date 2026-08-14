using DNDTracker.Vocabulary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNDTracker.Outbound.PostgresDb.Database.Postgres.Configuration;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItemModel>
{
    public void Configure(EntityTypeBuilder<InventoryItemModel> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedNever();
    }
}
