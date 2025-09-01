using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content).IsRequired().HasMaxLength(2000);

            // Relationship to User (Author)
            builder.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent user deletion if they have comments

            // Relationship to Inventory
            builder.HasOne(c => c.Inventory)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade); // Delete comments if inventory is deleted
        }
    }
}