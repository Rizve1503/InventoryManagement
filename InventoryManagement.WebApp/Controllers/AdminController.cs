using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagement.WebApp.Controllers
{
    [Authorize(Roles = "Admin")] // Only users with the "Admin" role can access this controller
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.Name : "N/A",
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();

            var model = new UserListViewModel { Users = users };
            return View(model);
        }

        // POST: /Admin/ToggleBlockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlockUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsBlocked = !user.IsBlocked;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/ToggleAdminRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdminRole(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Admin");
            var userRole = await _context.Roles.FirstAsync(r => r.Name == "User");

            if (user != null)
            {
                // Toggle the role
                user.RoleId = user.RoleId == adminRole.Id ? userRole.Id : adminRole.Id;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId == currentUserId)
            {
                // Prevent admin from deleting themselves
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }
    }
}