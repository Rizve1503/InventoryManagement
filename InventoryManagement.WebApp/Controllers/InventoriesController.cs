using InventoryManagement.Application.Models.CustomId;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace InventoryManagement.WebApp.Controllers
{
    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomIdService _customIdService;
        private const int PageSize = 10; // Define page size

        public InventoriesController(ApplicationDbContext context, ICustomIdService customIdService)
        {
            _context = context;
            _customIdService = customIdService; 
        }

        // This action now only gets the FIRST page
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var userInventories = await _context.Inventories
                .Where(i => i.CreatorId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .Take(PageSize)
                .ToListAsync();

            // Used to determine if we need to show the loader and enable the script
            ViewBag.HasMoreInventories = (await _context.Inventories.CountAsync(i => i.CreatorId == userId)) > PageSize;

            return View(userInventories);
        }

        // NEW ACTION: This action will be called by AJAX to get subsequent pages
        [HttpGet]
        public async Task<IActionResult> GetInventoriesPage(int page = 2)
        {
            var userId = GetCurrentUserId();
            var userInventories = await _context.Inventories
                .Where(i => i.CreatorId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // If there's nothing to return, return an empty result
            if (!userInventories.Any())
            {
                return Content("");
            }

            return PartialView("_InventoryListPartial", userInventories);
        }

        // GET: /Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Inventories/Create (No changes to this action)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var inventory = new Inventory
                {
                    Title = model.Title,
                    Description = model.Description,
                    IsPublic = model.IsPublic,
                    CreatorId = GetCurrentUserId(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Inventories/Edit/5 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var inventory = await _context.Inventories.FindAsync(id);

            var inventory = await _context.Inventories
                            .Include(i => i.Tags)
                            .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            if (inventory.CreatorId != GetCurrentUserId())
            {
                return Forbid();
            }

            var model = new InventoryViewModel
            {
                Id = inventory.Id,
                Title = inventory.Title,
                Description = inventory.Description,
                IsPublic = inventory.IsPublic,
                RowVersion = inventory.RowVersion,
                Tags = string.Join(",", inventory.Tags.Select(t => t.Name))

            };

            return View(model);
        }

        // GET: /Inventories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Creator)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            // Pass the inventory to the view
            return View(inventory);
        }
        // POST: /Inventories/Edit/5 (No changes to this action)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var inventoryToUpdate = await _context.Inventories.FindAsync(id);

            var inventoryToUpdate = await _context.Inventories
                        .Include(i => i.Tags)
                        .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "The inventory you are trying to edit has been deleted.");
                return View(model);
            }

            if (inventoryToUpdate.CreatorId != GetCurrentUserId())
            {
                return Forbid();
            }

            _context.Entry(inventoryToUpdate).Property("RowVersion").OriginalValue = model.RowVersion;

            inventoryToUpdate.Title = model.Title;
            inventoryToUpdate.Description = model.Description;
            inventoryToUpdate.IsPublic = model.IsPublic;
            inventoryToUpdate.UpdatedAt = DateTime.UtcNow;

            await UpdateInventoryTags(inventoryToUpdate, model.Tags);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Inventory)entry.Entity;
                var databaseEntry = await entry.GetDatabaseValuesAsync();

                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save. The inventory was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Inventory)databaseEntry.ToObject();
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                        + "was modified by another user after you got the original value. Your edit operation "
                        + "was canceled. Review the current values in the database and try again.");
                    model.RowVersion = databaseValues.RowVersion;
                }
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


        // GET: /Inventories/Fields/5
        [HttpGet]
        public async Task<IActionResult> Fields(int? id)
        {
            if (id == null) return NotFound();

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return NotFound();

            // Authorization Check
            if (inventory.CreatorId != GetCurrentUserId()) return Forbid();

            var model = new CustomFieldsViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                RowVersion = inventory.RowVersion,
                // Map all 15 field states and names from entity to viewmodel
                CustomString1State = inventory.CustomString1State,
                CustomString1Name = inventory.CustomString1Name,
                // ... (repeat for all other custom fields) ...
                CustomLink3State = inventory.CustomLink3State,
                CustomLink3Name = inventory.CustomLink3Name
            };

            return View(model);
        }

        // POST: /Inventories/Fields/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fields(int id, CustomFieldsViewModel model)
        {
            if (id != model.InventoryId) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var inventoryToUpdate = await _context.Inventories.FindAsync(id);
            if (inventoryToUpdate == null) return NotFound();

            // Authorization Check
            if (inventoryToUpdate.CreatorId != GetCurrentUserId()) return Forbid();

            _context.Entry(inventoryToUpdate).Property("RowVersion").OriginalValue = model.RowVersion;

            // Map all 15 field states and names from viewmodel back to entity
            inventoryToUpdate.CustomString1State = model.CustomString1State;
            inventoryToUpdate.CustomString1Name = model.CustomString1State ? model.CustomString1Name : string.Empty;
            // ... (repeat for all other custom fields, ensuring Name is cleared if State is false) ...
            inventoryToUpdate.CustomLink3State = model.CustomLink3State;
            inventoryToUpdate.CustomLink3Name = model.CustomLink3State ? model.CustomLink3Name : string.Empty;

            inventoryToUpdate.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                // Add a success message to TempData
                TempData["SuccessMessage"] = "Custom fields saved successfully!";
                return RedirectToAction(nameof(Fields), new { id = model.InventoryId });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency error similar to the Edit action
                ModelState.AddModelError(string.Empty, "These settings were modified by another user. Please reload and try again.");
                return View(model);
            }
        }

        // GET: /Inventories/CustomId/5
        public async Task<IActionResult> CustomId(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return NotFound();

            if (inventory.CreatorId != GetCurrentUserId()) return Forbid();

            var model = new CustomIdViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                RowVersion = inventory.RowVersion
            };

            if (!string.IsNullOrEmpty(inventory.CustomIdFormatJson))
            {
                // Define the options
                var jsonOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                // ** THE FIX: Pass 'jsonOptions' as the second argument **
                model.Configuration = JsonSerializer.Deserialize<CustomIdConfiguration>(inventory.CustomIdFormatJson, jsonOptions) ?? new CustomIdConfiguration();

                model.IdFormatJson = inventory.CustomIdFormatJson;
            }

            return View(model);
        }

        // POST: /Inventories/CustomId/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomId(CustomIdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate config if validation fails to redisplay the editor
                if (!string.IsNullOrEmpty(model.IdFormatJson))
                {
                    // --- ADD THESE OPTIONS ---
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() }
                    };
                    // --- USE THE OPTIONS IN THE CALL ---
                    model.Configuration = JsonSerializer.Deserialize<CustomIdConfiguration>(model.IdFormatJson, jsonOptions) ?? new CustomIdConfiguration();
                }
                return View(model);
            }

            var inventoryToUpdate = await _context.Inventories.FindAsync(model.InventoryId);
            if (inventoryToUpdate == null) return NotFound();

            if (inventoryToUpdate.CreatorId != GetCurrentUserId()) return Forbid();

            _context.Entry(inventoryToUpdate).Property("RowVersion").OriginalValue = model.RowVersion;
            inventoryToUpdate.CustomIdFormatJson = model.IdFormatJson;
            inventoryToUpdate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Custom ID format saved successfully!";
            return RedirectToAction(nameof(CustomId), new { id = model.InventoryId });
        }

        // POST: /Inventories/GeneratePreviewId
        [HttpPost]
        public async Task<IActionResult> GeneratePreviewId(int inventoryId, [FromBody] CustomIdConfiguration config)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();

            // Temporarily apply the new format for preview generation
            inventory.CustomIdFormatJson = JsonSerializer.Serialize(config);

            var previewId = await _customIdService.GeneratePreviewIdAsync(inventory);
            return Ok(new { id = previewId });
        }

        private async Task UpdateInventoryTags(Inventory inventory, string? tagsString)
        {
            // Clear existing tags
            inventory.Tags.Clear();

            if (string.IsNullOrWhiteSpace(tagsString))
            {
                return;
            }

            // Split the input string into individual tag names
            var tagNames = tagsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(t => t.Trim().ToLower())
                                     .Distinct();

            foreach (var tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName)) continue;

                // Check if the tag already exists in the database
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (existingTag != null)
                {
                    // If it exists, add it to the inventory
                    inventory.Tags.Add(existingTag);
                }
                else
                {
                    // If it's a new tag, create it and add it
                    var newTag = new Tag { Name = tagName };
                    inventory.Tags.Add(newTag);
                    // EF Core will automatically add the new tag to the Tags table
                }
            }
        }

        // GET: /Inventories/SearchTags?term=...
        [HttpGet]
        public async Task<IActionResult> SearchTags(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var tags = await _context.Tags
                    .Where(t => t.Name.ToLower().StartsWith(term.ToLower()))
                    .Select(t => t.Name)
                    .ToListAsync();
                return Json(tags);
            }
            return Json(new List<string>());
        }
    }
}