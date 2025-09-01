using System.Collections.Generic;

namespace InventoryManagement.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for the many-to-many relationship
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}