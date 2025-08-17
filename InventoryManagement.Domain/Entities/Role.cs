namespace InventoryManagement.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for the users in this role
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}