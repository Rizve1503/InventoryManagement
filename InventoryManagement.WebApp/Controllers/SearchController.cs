using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.WebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Search?query=...
        public async Task<IActionResult> Index(string query)
        {
            var model = new SearchViewModel { Query = query };

            if (!string.IsNullOrWhiteSpace(query))
            {
                string searchTerm = $"\"{query}*\"";

                // Search Inventories using CONTAINS for more precise matching
                model.FoundInventories = await _context.Inventories
                    .Where(i => EF.Functions.Contains(i.Title, searchTerm) || EF.Functions.Contains(i.Description, searchTerm))
                    .ToListAsync();

                // Search Items and include their parent Inventory for context
                model.FoundItems = await _context.Items
                    .Include(i => i.Inventory)
                    .Where(i =>
                        EF.Functions.Contains(i.CustomString1Value, searchTerm) ||
                        EF.Functions.Contains(i.CustomString2Value, searchTerm) ||
                        EF.Functions.Contains(i.CustomString3Value, searchTerm) ||
                        EF.Functions.Contains(i.CustomText1Value, searchTerm) ||
                        EF.Functions.Contains(i.CustomText2Value, searchTerm) ||
                        EF.Functions.Contains(i.CustomText3Value, searchTerm)
                    )
                    .Select(item => new ItemSearchResult
                    {
                        Item = item,
                        InventoryTitle = item.Inventory != null ? item.Inventory.Title : "N/A",
                        InventoryId = item.InventoryId
                    })
                    .ToListAsync();
            }

            return View(model);
        }

        // GET: /Search/Tag?tagName=...
        [HttpGet]
        public async Task<IActionResult> Tag(string tagName)
        {
            var model = new SearchViewModel
            {
                Query = $"Tag: {tagName}"
            };

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                // Find all inventories that have a tag with the specified name
                model.FoundInventories = await _context.Inventories
                    .Where(i => i.Tags.Any(t => t.Name == tagName))
                    .ToListAsync();
            }

            // Reuse the main search Index view to display the results
            return View("Index", model);
        }
    }
}