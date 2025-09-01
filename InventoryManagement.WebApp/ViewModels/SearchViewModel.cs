using InventoryManagement.Domain.Entities;
using System.Collections.Generic;

namespace InventoryManagement.WebApp.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Inventory> FoundInventories { get; set; } = new List<Inventory>();
        public List<ItemSearchResult> FoundItems { get; set; } = new List<ItemSearchResult>();
    }

    // A special class to hold item search results with their parent inventory's title
    public class ItemSearchResult
    {
        public Item Item { get; set; } = new();
        public string InventoryTitle { get; set; } = string.Empty;
        public int InventoryId { get; set; }
    }
}