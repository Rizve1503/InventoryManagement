using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq; 
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;

namespace InventoryManagement.WebApp.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomIdService _customIdService;
        private readonly IHtmlLocalizer<InventoriesController> _inventoriesLocalizer;

        public ItemsController(ApplicationDbContext context, ICustomIdService customIdService, IHtmlLocalizer<InventoriesController> inventoriesLocalizer)
        {
            _context = context;
            _customIdService = customIdService;
            _inventoriesLocalizer = inventoriesLocalizer;
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
        [HttpGet]
        public async Task<IActionResult> Create(int inventoryId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();
            if (!inventory.IsPublic && inventory.CreatorId != currentUserId)
            {
                return Forbid();
            }

            ViewData["Title"] = _inventoriesLocalizer["Add New Item to {0}", inventory.Title];

            var model = new ItemViewModel
            {
                InventoryId = inventoryId,
                Inventory = inventory, // This still provides the full inventory context
                Item = new Item()
            };

            // Create a dictionary of all possible fields for easy lookup
            var allFields = new Dictionary<string, CustomFieldViewModel>
            {
                {"cs1", new CustomFieldViewModel { FieldKey = "cs1", FieldType = "Single-Line Text", State = inventory.CustomString1State, Name = inventory.CustomString1Name, MaxLength = inventory.CustomString1MaxLength, Regex = inventory.CustomString1Regex }},
                {"cs2", new CustomFieldViewModel { FieldKey = "cs2", FieldType = "Single-Line Text", State = inventory.CustomString2State, Name = inventory.CustomString2Name, MaxLength = inventory.CustomString2MaxLength, Regex = inventory.CustomString2Regex }},
                {"cs3", new CustomFieldViewModel { FieldKey = "cs3", FieldType = "Single-Line Text", State = inventory.CustomString3State, Name = inventory.CustomString3Name, MaxLength = inventory.CustomString3MaxLength, Regex = inventory.CustomString3Regex }},
                {"ct1", new CustomFieldViewModel { FieldKey = "ct1", FieldType = "Multi-Line Text", State = inventory.CustomText1State, Name = inventory.CustomText1Name }},
                {"ct2", new CustomFieldViewModel { FieldKey = "ct2", FieldType = "Multi-Line Text", State = inventory.CustomText2State, Name = inventory.CustomText2Name }},
                {"ct3", new CustomFieldViewModel { FieldKey = "ct3", FieldType = "Multi-Line Text", State = inventory.CustomText3State, Name = inventory.CustomText3Name }},
                {"cn1", new CustomFieldViewModel { FieldKey = "cn1", FieldType = "Numeric", State = inventory.CustomNumeric1State, Name = inventory.CustomNumeric1Name, MinValue = inventory.CustomNumeric1MinValue, MaxValue = inventory.CustomNumeric1MaxValue }},
                {"cn2", new CustomFieldViewModel { FieldKey = "cn2", FieldType = "Numeric", State = inventory.CustomNumeric2State, Name = inventory.CustomNumeric2Name, MinValue = inventory.CustomNumeric2MinValue, MaxValue = inventory.CustomNumeric2MaxValue }},
                {"cn3", new CustomFieldViewModel { FieldKey = "cn3", FieldType = "Numeric", State = inventory.CustomNumeric3State, Name = inventory.CustomNumeric3Name, MinValue = inventory.CustomNumeric3MinValue, MaxValue = inventory.CustomNumeric3MaxValue }},
                {"cb1", new CustomFieldViewModel { FieldKey = "cb1", FieldType = "Checkbox (Yes/No)", State = inventory.CustomBool1State, Name = inventory.CustomBool1Name }},
                {"cb2", new CustomFieldViewModel { FieldKey = "cb2", FieldType = "Checkbox (Yes/No)", State = inventory.CustomBool2State, Name = inventory.CustomBool2Name }},
                {"cb3", new CustomFieldViewModel { FieldKey = "cb3", FieldType = "Checkbox (Yes/No)", State = inventory.CustomBool3State, Name = inventory.CustomBool3Name }},
                {"cl1", new CustomFieldViewModel { FieldKey = "cl1", FieldType = "Document/Image Link", State = inventory.CustomLink1State, Name = inventory.CustomLink1Name }},
                {"cl2", new CustomFieldViewModel { FieldKey = "cl2", FieldType = "Document/Image Link", State = inventory.CustomLink2State, Name = inventory.CustomLink2Name }},
                {"cl3", new CustomFieldViewModel { FieldKey = "cl3", FieldType = "Document/Image Link", State = inventory.CustomLink3State, Name = inventory.CustomLink3Name }},
                {"csel1", new CustomFieldViewModel { FieldKey = "csel1", FieldType = "Select from List", State = inventory.CustomSelect1State, Name = inventory.CustomSelect1Name, Options = inventory.CustomSelect1Options }},
                {"csel2", new CustomFieldViewModel { FieldKey = "csel2", FieldType = "Select from List", State = inventory.CustomSelect2State, Name = inventory.CustomSelect2Name, Options = inventory.CustomSelect2Options }},
                {"csel3", new CustomFieldViewModel { FieldKey = "csel3", FieldType = "Select from List", State = inventory.CustomSelect3State, Name = inventory.CustomSelect3Name, Options = inventory.CustomSelect3Options }},
            };

            // Determine the order
            if (!string.IsNullOrEmpty(inventory.CustomFieldOrderJson))
            {
                var orderedKeys = JsonSerializer.Deserialize<List<string>>(inventory.CustomFieldOrderJson);
                if (orderedKeys != null)
                {
                    foreach (var key in orderedKeys)
                    {
                        if (allFields.ContainsKey(key))
                        {
                            model.OrderedFields.Add(allFields[key]);
                            allFields.Remove(key); // Remove to avoid duplication
                        }
                    }
                }
            }

            // Add any remaining fields that weren't in the saved order
            model.OrderedFields.AddRange(allFields.Values);

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