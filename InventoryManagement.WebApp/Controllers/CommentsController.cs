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
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Comments/GetCommentsForInventory?inventoryId=5
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCommentsForInventory(int inventoryId)
        {
            var comments = await _context.Comments
                .Include(c => c.Author) // Eager load the author's name and details
                .Where(c => c.InventoryId == inventoryId)
                .OrderBy(c => c.CreatedAt) // Show oldest comments first
                .ToListAsync();

            // Returns an HTML snippet containing the list of comments
            return PartialView("_CommentListPartial", comments);
        }

        // POST: /Comments/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel model)
        {
            var inventory = await _context.Inventories.FindAsync(model.InventoryId);

            if (inventory == null)
            {
                return NotFound();
            }

            if (!inventory.IsPublic)
            {
                // If the inventory is not public, only the creator can comment.
                if (inventory.CreatorId != GetCurrentUserId())
                {
                    return Forbid(); // Return a forbidden status
                }
            }

            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = model.Content,
                    InventoryId = model.InventoryId,
                    AuthorId = GetCurrentUserId(),
                    CreatedAt = System.DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // We must load the Author navigation property before sending the new
                // comment back to the client, so it can display the author's name.
                await _context.Entry(comment).Reference(c => c.Author).LoadAsync();

                // Return an HTML snippet of JUST the new comment to be appended by JavaScript
                return PartialView("_CommentSinglePartial", comment);
            }

            // If the model is not valid (e.g., empty content), return a Bad Request status.
            // This will trigger the .error() or .fail() in the jQuery AJAX call.
            return BadRequest(ModelState);
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out int userId))
            {
                return userId;
            }
            // This should not happen for an authorized user, but it's a safeguard.
            throw new InvalidOperationException("User ID is not available in the claims.");
        }
    }
}