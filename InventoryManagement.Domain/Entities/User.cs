namespace InventoryManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key for the Role
        public int RoleId { get; set; }
        // Navigation property to the Role
        public virtual Role? Role { get; set; }
    }
}