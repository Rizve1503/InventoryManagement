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
        private readonly IFileStorageService _fileStorageService;
        private const int PageSize = 10; // Define page size
        public InventoriesController(ApplicationDbContext context, ICustomIdService customIdService, IFileStorageService fileStorageService)
        {
            _context = context;
            _customIdService = customIdService;
            _fileStorageService = fileStorageService; // Add this
        }
        // This action now only gets the FIRST page
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            string viewType = Request.Cookies["InventoryViewType"] ?? "list";
            ViewBag.ViewType = viewType;
            var userInventories = await _context.Inventories
            .Include(i => i.Items) // Eager load Items for the count
            .Where(i => i.CreatorId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Take(PageSize)
            .ToListAsync();
            ViewBag.HasMoreInventories = (await _context.Inventories.CountAsync(i => i.CreatorId == userId)) > PageSize;
            return View(userInventories);
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoriesPage(int page = 2, string viewType = "list")
        {
            var userId = GetCurrentUserId();
            var userInventories = await _context.Inventories
            .Include(i => i.Items)
            .Where(i => i.CreatorId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
            if (!userInventories.Any())
            {
                return Content("");
            }
            // This line correctly determines the partial view name
            string partialViewName = viewType == "card" ? "_InventoryCardPartial" : "_InventoryListPartial";
            ViewBag.CurrentUserId = GetCurrentUserId();
            // ** THE FIX: Use the 'partialViewName' variable here. **
            return PartialView(partialViewName, userInventories);
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
                Tags = string.Join(",", inventory.Tags.Select(t => t.Name)),
                ExistingImageUrl = inventory.ImageUrl
            };
            return View(model);
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
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // 1. Delete the old image from Cloudinary if it exists
                if (!string.IsNullOrEmpty(inventoryToUpdate.ImageUrl))
                {
                    _fileStorageService.DeleteFile(inventoryToUpdate.ImageUrl);
                }
                // 2. Save the new image to Cloudinary and get the new URL
                inventoryToUpdate.ImageUrl = await _fileStorageService.SaveFileAsync(model.ImageFile);
            }
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
        [AllowAnonymous]
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
        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out int userId))
            {
                return userId;
            }
            // throw new InvalidOperationException("User ID is not available in the claims.");
            return 0; // Return a default value (e.g., 0) instead of null for non-nullable int
        }
        // GET: /Inventories/Fields/5
        [HttpGet]
        public async Task<IActionResult> Fields(int? id)
        {
            if (id == null) return NotFound();
            var inventory = await _context.Inventories
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
            if (inventory == null) return NotFound();
            // Authorization Check
            if (inventory.CreatorId != GetCurrentUserId()) return Forbid();
            var model = new CustomFieldsViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                RowVersion = inventory.RowVersion,
                // Single-line text fields
                CustomString1State = inventory.CustomString1State,
                CustomString1Name = inventory.CustomString1Name,
                CustomString2State = inventory.CustomString2State,
                CustomString2Name = inventory.CustomString2Name,
                CustomString3State = inventory.CustomString3State,
                CustomString3Name = inventory.CustomString3Name,
                // Multi-line text fields
                CustomText1State = inventory.CustomText1State,
                CustomText1Name = inventory.CustomText1Name,
                CustomText2State = inventory.CustomText2State,
                CustomText2Name = inventory.CustomText2Name,
                CustomText3State = inventory.CustomText3State,
                CustomText3Name = inventory.CustomText3Name,
                // Numeric fields
                CustomNumeric1State = inventory.CustomNumeric1State,
                CustomNumeric1Name = inventory.CustomNumeric1Name,
                CustomNumeric2State = inventory.CustomNumeric2State,
                CustomNumeric2Name = inventory.CustomNumeric2Name,
                CustomNumeric3State = inventory.CustomNumeric3State,
                CustomNumeric3Name = inventory.CustomNumeric3Name,
                // Boolean (checkbox) fields
                CustomBool1State = inventory.CustomBool1State,
                CustomBool1Name = inventory.CustomBool1Name,
                CustomBool2State = inventory.CustomBool2State,
                CustomBool2Name = inventory.CustomBool2Name,
                CustomBool3State = inventory.CustomBool3State,
                CustomBool3Name = inventory.CustomBool3Name,
                // Document/Image Link fields
                CustomLink1State = inventory.CustomLink1State,
                CustomLink1Name = inventory.CustomLink1Name,
                CustomLink2State = inventory.CustomLink2State,
                CustomLink2Name = inventory.CustomLink2Name,
                CustomLink3State = inventory.CustomLink3State,
                CustomLink3Name = inventory.CustomLink3Name,
                // Select from List fields
                CustomSelect1State = inventory.CustomSelect1State,
                CustomSelect1Name = inventory.CustomSelect1Name,
                CustomSelect1Options = inventory.CustomSelect1Options,
                CustomSelect2State = inventory.CustomSelect2State,
                CustomSelect2Name = inventory.CustomSelect2Name,
                CustomSelect2Options = inventory.CustomSelect2Options,
                CustomSelect3State = inventory.CustomSelect3State,
                CustomSelect3Name = inventory.CustomSelect3Name,
                CustomSelect3Options = inventory.CustomSelect3Options
            };
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStatsForInventory(int inventoryId)
        {
            var inventory = await _context.Inventories
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inventory == null) return NotFound();
            var items = await _context.Items
            .AsNoTracking()
            .Where(i => i.InventoryId == inventoryId)
            .ToListAsync();
            var model = new InventoryStatsViewModel
            {
                TotalItems = items.Count
            };
            if (items.Any())
            {
                // --- Calculate Numeric Stats (Fully Implemented) ---
                if (inventory.CustomNumeric1State)
                {
                    var values = items.Select(i => i.CustomNumeric1Value).Where(v => v.HasValue).Select(v => v.Value);
                    if (values.Any())
                    {
                        model.NumericStats.Add(new NumericStat { FieldName = inventory.CustomNumeric1Name, Average = values.Average()!, Min = values.Min()!, Max = values.Max()! });
                    }
                }
                if (inventory.CustomNumeric2State)
                {
                    var values = items.Select(i => i.CustomNumeric2Value).Where(v => v.HasValue).Select(v => v.Value);
                    if (values.Any())
                    {
                        model.NumericStats.Add(new NumericStat { FieldName = inventory.CustomNumeric2Name, Average = values.Average()!, Min = values.Min()!, Max = values.Max()! });
                    }
                }
                if (inventory.CustomNumeric3State)
                {
                    var values = items.Select(i => i.CustomNumeric3Value).Where(v => v.HasValue).Select(v => v.Value);
                    if (values.Any())
                    {
                        model.NumericStats.Add(new NumericStat { FieldName = inventory.CustomNumeric3Name, Average = values.Average()!, Min = values.Min()!, Max = values.Max()! });
                    }
                }
                // --- Calculate String Stats (Fully Implemented) ---
                if (inventory.CustomString1State)
                {
                    var stat = items.Select(i => i.CustomString1Value).Where(s => !string.IsNullOrEmpty(s))
                    .GroupBy(s => s)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                    if (stat.Any()) model.StringStats.Add(new StringStat { FieldName = inventory.CustomString1Name, TopValues = stat.ToDictionary(s => s.Value!, s => s.Count) });
                }
                if (inventory.CustomString2State)
                {
                    var stat = items.Select(i => i.CustomString2Value).Where(s => !string.IsNullOrEmpty(s))
                    .GroupBy(s => s)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                    if (stat.Any()) model.StringStats.Add(new StringStat { FieldName = inventory.CustomString2Name, TopValues = stat.ToDictionary(s => s.Value!, s => s.Count) });
                }
                if (inventory.CustomString3State)
                {
                    var stat = items.Select(i => i.CustomString3Value).Where(s => !string.IsNullOrEmpty(s))
                    .GroupBy(s => s)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                    if (stat.Any()) model.StringStats.Add(new StringStat { FieldName = inventory.CustomString3Name, TopValues = stat.ToDictionary(s => s.Value!, s => s.Count) });
                }
            }
            return new JsonResult(model);
        }
        // POST: /Inventories/Fields/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fields(int id, CustomFieldsViewModel model)
        {
            if (id != model.InventoryId)
            {
                return NotFound();
            }
            // We must load the entity fresh from the database to ensure we have the latest version to update.
            var inventoryToUpdate = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
            if (inventoryToUpdate == null)
            {
                // This can happen if another user deleted the inventory.
                TempData["ErrorMessage"] = "The inventory you tried to edit was deleted.";
                return RedirectToAction(nameof(Index));
            }
            // Authorization Check
            if (inventoryToUpdate.CreatorId != GetCurrentUserId())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                model.InventoryTitle = inventoryToUpdate.Title; // Repopulate title on error
                return View(model);
            }
            // --- Start of Complete and Correct Property Mapping ---
            // Single-line text fields
            inventoryToUpdate.CustomString1State = model.CustomString1State;
            inventoryToUpdate.CustomString1Name = model.CustomString1State ? (model.CustomString1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomString2State = model.CustomString2State;
            inventoryToUpdate.CustomString2Name = model.CustomString2State ? (model.CustomString2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomString3State = model.CustomString3State;
            inventoryToUpdate.CustomString3Name = model.CustomString3State ? (model.CustomString3Name ?? string.Empty) : string.Empty;
            // Multi-line text fields
            inventoryToUpdate.CustomText1State = model.CustomText1State;
            inventoryToUpdate.CustomText1Name = model.CustomText1State ? (model.CustomText1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomText2State = model.CustomText2State;
            inventoryToUpdate.CustomText2Name = model.CustomText2State ? (model.CustomText2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomText3State = model.CustomText3State;
            inventoryToUpdate.CustomText3Name = model.CustomText3State ? (model.CustomText3Name ?? string.Empty) : string.Empty;
            // Numeric fields
            inventoryToUpdate.CustomNumeric1State = model.CustomNumeric1State;
            inventoryToUpdate.CustomNumeric1Name = model.CustomNumeric1State ? (model.CustomNumeric1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomNumeric2State = model.CustomNumeric2State;
            inventoryToUpdate.CustomNumeric2Name = model.CustomNumeric2State ? (model.CustomNumeric2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomNumeric3State = model.CustomNumeric3State;
            inventoryToUpdate.CustomNumeric3Name = model.CustomNumeric3State ? (model.CustomNumeric3Name ?? string.Empty) : string.Empty;
            // Boolean (checkbox) fields
            inventoryToUpdate.CustomBool1State = model.CustomBool1State;
            inventoryToUpdate.CustomBool1Name = model.CustomBool1State ? (model.CustomBool1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomBool2State = model.CustomBool2State;
            inventoryToUpdate.CustomBool2Name = model.CustomBool2State ? (model.CustomBool2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomBool3State = model.CustomBool3State;
            inventoryToUpdate.CustomBool3Name = model.CustomBool3State ? (model.CustomBool3Name ?? string.Empty) : string.Empty;
            // Document/Image Link fields
            inventoryToUpdate.CustomLink1State = model.CustomLink1State;
            inventoryToUpdate.CustomLink1Name = model.CustomLink1State ? (model.CustomLink1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomLink2State = model.CustomLink2State;
            inventoryToUpdate.CustomLink2Name = model.CustomLink2State ? (model.CustomLink2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomLink3State = model.CustomLink3State;
            inventoryToUpdate.CustomLink3Name = model.CustomLink3State ? (model.CustomLink3Name ?? string.Empty) : string.Empty;
            // Select from List fields
            inventoryToUpdate.CustomSelect1State = model.CustomSelect1State;
            inventoryToUpdate.CustomSelect1Name = model.CustomSelect1State ? (model.CustomSelect1Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomSelect1Options = model.CustomSelect1State ? (model.CustomSelect1Options ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomSelect2State = model.CustomSelect2State;
            inventoryToUpdate.CustomSelect2Name = model.CustomSelect2State ? (model.CustomSelect2Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomSelect2Options = model.CustomSelect2State ? (model.CustomSelect2Options ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomSelect3State = model.CustomSelect3State;
            inventoryToUpdate.CustomSelect3Name = model.CustomSelect3State ? (model.CustomSelect3Name ?? string.Empty) : string.Empty;
            inventoryToUpdate.CustomSelect3Options = model.CustomSelect3State ? (model.CustomSelect3Options ?? string.Empty) : string.Empty;
            // --- End of Complete Property Mapping ---
            inventoryToUpdate.UpdatedAt = DateTime.UtcNow;
            try
            {
                // Tell EF that the entity has been modified.
                _context.Inventories.Update(inventoryToUpdate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Custom fields saved successfully!";
                return RedirectToAction(nameof(Fields), new { id = model.InventoryId });
            }
            catch (DbUpdateConcurrencyException)
            {
                // This error is now less likely but is good practice to keep.
                ModelState.AddModelError(string.Empty, "These settings were modified by another user. Please reload and try again.");
                model.InventoryTitle = inventoryToUpdate.Title;
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
        // GET: /Inventories/GetItemsForInventory?inventoryId=5
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetItemsForInventory(int inventoryId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();
            var items = await _context.Items
            .Include(i => i.Likes) // Eager load the likes
            .Where(i => i.InventoryId == inventoryId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
            // Pass the current user ID to the view to determine if they've liked each item
            ViewBag.CurrentUserId = GetCurrentUserId();
            ViewBag.Inventory = inventory;
            return PartialView("_ItemListPartial", items);
        }
        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inventory = await _context.Inventories
            .Include(i => i.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (inventory == null) return NotFound();
            // Authorization Check
            if (inventory.CreatorId != GetCurrentUserId()) return Forbid();
            return View(inventory);
        }
        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return NotFound();
            // Authorization Check
            if (inventory.CreatorId != GetCurrentUserId()) return Forbid();
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}