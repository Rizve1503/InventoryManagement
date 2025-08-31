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
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10; // Define page size

        public InventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MODIFIED: This action now only gets the FIRST page
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

        // GET: /Inventories/Edit/5 (No changes to this action)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FindAsync(id);

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
                RowVersion = inventory.RowVersion
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

            var inventoryToUpdate = await _context.Inventories.FindAsync(id);

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
    }
}