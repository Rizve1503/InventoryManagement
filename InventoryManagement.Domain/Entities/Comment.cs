using System;

namespace InventoryManagement.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Relationships ---
        public int AuthorId { get; set; }
        public virtual User? Author { get; set; }

        public int InventoryId { get; set; }
        public virtual Inventory? Inventory { get; set; }
    }
}