using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace InventoryManagement.WebApp.ViewModels
{
    // This class represents a SINGLE custom field card in the UI
    public class CustomFieldViewModel
    {
        // A unique key to identify the field (e.g., "cs1", "cn2")
        public string FieldKey { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;

        public bool State { get; set; }
        [StringLength(100)] public string? Name { get; set; }

        // Options for "Select from List"
        [StringLength(1000)] public string? Options { get; set; }

        // Validation rules
        [Range(1, 4000)] public int? MaxLength { get; set; }
        [StringLength(200)] public string? Regex { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
    }

    // This is the main ViewModel for the entire "Fields" page
    public class CustomFieldsPageViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;
        public byte[]? RowVersion { get; set; }

        // This will hold the ordered list of fields for rendering and model binding
        public List<CustomFieldViewModel> Fields { get; set; } = new List<CustomFieldViewModel>();

        // This will receive the new order from the JavaScript
        public string? FieldOrderJson { get; set; }
    }
}