using InventoryManagement.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                // Get the 5 most recently created inventories
                LatestInventories = await _context.Inventories
                    .Where(i => i.IsPublic)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(5)
                    .ToListAsync(),

                // Get the top 5 inventories based on the number of items they contain
                TopInventories = await _context.Inventories
                    .Include(i => i.Items)
                    .Where(i => i.IsPublic)
                    .OrderByDescending(i => i.Items.Count)
                    .Take(5)
                    .ToListAsync(),

                // Get all tags and count how many inventories are associated with each
                TagCloud = await _context.Tags
                    .Where(t => t.Inventories.Any()) // Only include tags that are actually used
                    .Select(t => new TagCloudItem
                    {
                        TagName = t.Name,
                        InventoryCount = t.Inventories.Count
                    })
                    .OrderByDescending(t => t.InventoryCount) // Show most popular tags first
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
