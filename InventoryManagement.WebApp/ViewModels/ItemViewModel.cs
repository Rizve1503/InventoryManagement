using InventoryManagement.Domain.Entities;

namespace InventoryManagement.WebApp.ViewModels
{
    // This ViewModel is used for displaying and creating items.
    // It includes the inventory details to dynamically build views.
    public class ItemViewModel
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; } // To access field definitions
       // public Item? Item { get; set; } // The actual item data for editing or display
        public Item Item { get; set; } = new();
    }

    // A specific ViewModel for the Item Index page
    public class ItemIndexViewModel
    {
        public Inventory Inventory { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}