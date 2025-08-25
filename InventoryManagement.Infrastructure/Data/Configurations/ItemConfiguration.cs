using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.Id);

            // This is a critical requirement: CustomID must be unique WITHIN an inventory.
            // A composite index on InventoryId and CustomId enforces this at the database level.
            // We add a filter to ignore null CustomIds, allowing items to be created before an ID is assigned.
            builder.HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique()
                .HasFilter("[CustomId] IS NOT NULL");

            // Configure relationship to Inventory
            builder.HasOne(i => i.Inventory)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InventoryId)
                .OnDelete(DeleteBehavior.Cascade); // If an inventory is deleted, all its items are deleted too.

            // Configure precision for numeric fields
            builder.Property(i => i.CustomNumeric1Value).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric2Value).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric3Value).HasPrecision(18, 4);

            // Configure RowVersion for optimistic concurrency
            builder.Property(i => i.RowVersion)
                .IsRowVersion();
        }
    }
}
