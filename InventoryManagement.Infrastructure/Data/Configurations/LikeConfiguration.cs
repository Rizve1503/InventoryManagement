using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            // Define the composite primary key. This is the crucial step that ensures
            // a user can only like an item once.
            builder.HasKey(l => new { l.UserId, l.ItemId });

            // Configure the relationship from Like to User
            builder.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their likes are deleted

            // Configure the relationship from Like to Item
            builder.HasOne(l => l.Item)
                .WithMany(i => i.Likes)
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Cascade); // If item is deleted, its likes are deleted
        }
    }
}