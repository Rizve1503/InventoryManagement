namespace InventoryManagement.Domain.Entities
{
    public class Like
    {
        // Composite Primary Key (UserId, ItemId)
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
    }
}