using InventoryManagement.Application.Services;
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
        private readonly ICustomIdService _customIdService;

        public ItemsController(ApplicationDbContext context, ICustomIdService customIdService)
        {
            _context = context;
            _customIdService = customIdService;
        }

        [AllowAnonymous]
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
            model.Inventory = inventory; // Re-assign for the view in case of error

            // Authorization Check
            var currentUserId = GetCurrentUserId();
            var isCreator = inventory.CreatorId == currentUserId;
            if (!isCreator && !inventory.IsPublic)
            {
                return Forbid();
            }

            // --- Start: Advanced Validation Logic ---
            if (ModelState.IsValid)
            {
                // String Field Validation
                if (inventory.CustomString1State) ValidateStringField(model.Item.CustomString1Value, inventory.CustomString1Name, inventory.CustomString1MaxLength, inventory.CustomString1Regex, "Item.CustomString1Value");
                if (inventory.CustomString2State) ValidateStringField(model.Item.CustomString2Value, inventory.CustomString2Name, inventory.CustomString2MaxLength, inventory.CustomString2Regex, "Item.CustomString2Value");
                if (inventory.CustomString3State) ValidateStringField(model.Item.CustomString3Value, inventory.CustomString3Name, inventory.CustomString3MaxLength, inventory.CustomString3Regex, "Item.CustomString3Value");

                // Numeric Field Validation
                if (inventory.CustomNumeric1State) ValidateNumericField(model.Item.CustomNumeric1Value, inventory.CustomNumeric1Name, inventory.CustomNumeric1MinValue, inventory.CustomNumeric1MaxValue, "Item.CustomNumeric1Value");
                if (inventory.CustomNumeric2State) ValidateNumericField(model.Item.CustomNumeric2Value, inventory.CustomNumeric2Name, inventory.CustomNumeric2MinValue, inventory.CustomNumeric2MaxValue, "Item.CustomNumeric2Value");
                if (inventory.CustomNumeric3State) ValidateNumericField(model.Item.CustomNumeric3Value, inventory.CustomNumeric3Name, inventory.CustomNumeric3MinValue, inventory.CustomNumeric3MaxValue, "Item.CustomNumeric3Value");
            }
            // --- End: Advanced Validation Logic ---

            if (ModelState.IsValid)
            {
                var itemToCreate = model.Item;

                // Generate Custom ID if format is defined
                if (!string.IsNullOrEmpty(inventory.CustomIdFormatJson))
                {
                    itemToCreate.CustomId = await _customIdService.GenerateIdAsync(inventory);
                }

                itemToCreate.InventoryId = model.InventoryId;
                itemToCreate.CreatedAt = DateTime.UtcNow;

                _context.Items.Add(itemToCreate);
                await _context.SaveChangesAsync();

                // Redirect to the Details page (tabbed view) after creation
                return RedirectToAction("Details", "Inventories", new { id = model.InventoryId });
            }

            // If we get here, something failed, redisplay form
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

        private void ValidateStringField(string? value, string fieldName, int? maxLength, string? regex, string propertyName)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (maxLength.HasValue && value.Length > maxLength.Value)
                {
                    ModelState.AddModelError(propertyName, $"{fieldName} cannot exceed {maxLength} characters.");
                }
                if (!string.IsNullOrEmpty(regex))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, regex))
                    {
                        ModelState.AddModelError(propertyName, $"{fieldName} does not match the required format.");
                    }
                }
            }
        }

        private void ValidateNumericField(decimal? value, string fieldName, decimal? minValue, decimal? maxValue, string propertyName)
        {
            if (value.HasValue)
            {
                if (minValue.HasValue && value < minValue.Value)
                {
                    ModelState.AddModelError(propertyName, $"{fieldName} must be at least {minValue}.");
                }
                if (maxValue.HasValue && value > maxValue.Value)
                {
                    ModelState.AddModelError(propertyName, $"{fieldName} must not exceed {maxValue}.");
                }
            }
        }
    }
}