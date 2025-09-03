using System.Collections.Generic;

namespace InventoryManagement.WebApp.ViewModels
{
    // For displaying a list of users
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }

    // For displaying and editing a single user
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsBlocked { get; set; } // We will add this column to the User entity
    }
}