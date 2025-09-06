using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagement.WebApp.Controllers
{
    [Authorize]
    [ApiController] // This makes the controller optimized for API responses
    [Route("api/[controller]")] // Sets the base route to /api/Likes
    public class LikesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/Likes/ToggleLike
        [HttpPost("ToggleLike")]
        public async Task<IActionResult> ToggleLike([FromBody] LikeRequest model)
        {
            if (model == null || model.ItemId <= 0)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();

            var itemToLike = await _context.Items
                            .Include(i => i.Inventory) // We need the parent inventory
                            .FirstOrDefaultAsync(i => i.Id == model.ItemId);

            if (itemToLike?.Inventory == null)
            {
                return NotFound();
            }

            // A user can like an item if its parent inventory is public.
            if (!itemToLike.Inventory.IsPublic)
            {
                // If not public, only the creator can like.
                if (itemToLike.Inventory.CreatorId != userId)
                {
                    return Forbid();
                }
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.ItemId == model.ItemId && l.UserId == userId);

            bool isLiked;

            if (existingLike != null)
            {
                // User has already liked the item, so unlike it.
                _context.Likes.Remove(existingLike);
                isLiked = false;
            }
            else
            {
                // User has not liked the item, so add a like.
                var newLike = new Like { ItemId = model.ItemId, UserId = userId };
                _context.Likes.Add(newLike);
                isLiked = true;
            }

            await _context.SaveChangesAsync();

            // Get the new total like count for the item
            var likeCount = await _context.Likes.CountAsync(l => l.ItemId == model.ItemId);

            // Return the new state
            return Ok(new { liked = isLiked, count = likeCount });
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // This will always succeed in an [Authorize] controller, but parsing is good practice.
            return int.Parse(userIdValue);
        }

        // A simple DTO for the request body
        public class LikeRequest
        {
            public int ItemId { get; set; }
        }
    }
}