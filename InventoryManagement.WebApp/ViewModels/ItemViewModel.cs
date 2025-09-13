using InventoryManagement.Domain.Entities;

namespace InventoryManagement.WebApp.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; } // To access field definitions
        public Item Item { get; set; } = new();
        public List<CustomFieldViewModel> OrderedFields { get; set; } = new List<CustomFieldViewModel>();

    }

    // A specific ViewModel for the Item Index page
    public class ItemIndexViewModel
    {
        public Inventory Inventory { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}