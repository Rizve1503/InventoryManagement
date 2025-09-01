using InventoryManagement.Application.Models.CustomId;
using System.Collections.Generic;

namespace InventoryManagement.WebApp.ViewModels
{
    public class CustomIdViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;
        public byte[]? RowVersion { get; set; }

        // This property will hold the JSON string from the form submission
        public string? IdFormatJson { get; set; }

        // This is for rendering the initial state of the editor
        public CustomIdConfiguration Configuration { get; set; } = new();
    }
}