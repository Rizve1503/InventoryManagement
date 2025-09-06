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
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Description)
                .HasMaxLength(2000);

            // Configure the relationship to the User (Creator)
            builder.HasOne(i => i.Creator)
                .WithMany(u => u.Inventories)
                .HasForeignKey(i => i.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a user if they own inventories

            // Configure RowVersion for optimistic concurrency
            builder.Property(i => i.RowVersion)
                .IsRowVersion();

            // Configure the many-to-many relationship with Tag
            // EF Core will automatically create a join table called 'InventoryTag'
            builder.HasMany(i => i.Tags)
                .WithMany(t => t.Inventories);

            // We use a precision of 18 and a scale of 4. This is a good general-purpose choice,
            // allowing for large numbers and up to four decimal places (e.g., 1234.5678).
            builder.Property(i => i.CustomNumeric1MinValue).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric1MaxValue).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric2MinValue).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric2MaxValue).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric3MinValue).HasPrecision(18, 4);
            builder.Property(i => i.CustomNumeric3MaxValue).HasPrecision(18, 4);

        }
    }
}
