using InventoryManagement.Domain.Entities;
using System.Collections.Generic;

namespace InventoryManagement.WebApp.ViewModels
{
    // A simple DTO for the tag cloud to hold the tag name and its frequency
    public class TagCloudItem
    {
        public string TagName { get; set; } = string.Empty;
        public int InventoryCount { get; set; }
    }

    public class HomeViewModel
    {
        public List<Inventory> LatestInventories { get; set; } = new List<Inventory>();
        public List<Inventory> TopInventories { get; set; } = new List<Inventory>();
        public List<TagCloudItem> TagCloud { get; set; } = new List<TagCloudItem>();
    }
}