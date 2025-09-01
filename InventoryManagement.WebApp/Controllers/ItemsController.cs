using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryManagement.WebApp.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Items/Index/5 (InventoryId)
        public async Task<IActionResult> Index(int inventoryId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return NotFound();
            }

            var model = new ItemIndexViewModel
            {
                Inventory = inventory,
                Items = inventory.Items.OrderByDescending(i => i.CreatedAt).ToList()
            };

            return View(model);
        }

        // GET: /Items/Create?inventoryId=5
        public async Task<IActionResult> Create(int inventoryId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            // --- Authorization Check ---
            // A user can add items if they are the creator OR the inventory is public.
            var isCreator = inventory.CreatorId == GetCurrentUserId();
            if (!isCreator && !inventory.IsPublic)
            {
                return Forbid();
            }

            var model = new ItemViewModel
            {
                InventoryId = inventoryId,
                Inventory = inventory,
                Item = new Item() // Initialize a new item for the form
            };

            return View(model);
        }

        // POST: /Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            var inventory = await _context.Inventories.FindAsync(model.InventoryId);
            if (inventory == null)
            {
                ModelState.AddModelError("", "Invalid Inventory.");
                return View(model);
            }

            // Re-assign for the view in case of error
            model.Inventory = inventory;

            // --- Authorization Check ---
            var isCreator = inventory.CreatorId == GetCurrentUserId();
            if (!isCreator && !inventory.IsPublic)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // We bind the Item property from the ViewModel
                var itemToCreate = model.Item;
                if (itemToCreate == null)
                {
                    // This is an unexpected error
                    return BadRequest("Item data is missing.");
                }

                itemToCreate.InventoryId = model.InventoryId;
                itemToCreate.CreatedAt = DateTime.UtcNow;

                _context.Items.Add(itemToCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { inventoryId = model.InventoryId });
            }

            return View(model);
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out int userId))
            {
                return userId;
            }
            throw new InvalidOperationException("User ID is not available in the claims.");
        }
    }
}