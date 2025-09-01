using InventoryManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebApp.ViewModels
{
    public class CustomFieldsViewModel
    {
        public int InventoryId { get; set; }

        [Required]
        public string InventoryTitle { get; set; } = string.Empty;

        // --- Concurrency ---
        public byte[]? RowVersion { get; set; }

        // --- Custom Field Definitions ---
        // Single-line text fields
        public bool CustomString1State { get; set; }
        [StringLength(100)] public string CustomString1Name { get; set; } = string.Empty;
        public bool CustomString2State { get; set; }
        [StringLength(100)] public string CustomString2Name { get; set; } = string.Empty;
        public bool CustomString3State { get; set; }
        [StringLength(100)] public string CustomString3Name { get; set; } = string.Empty;

        // Multi-line text fields
        public bool CustomText1State { get; set; }
        [StringLength(100)] public string CustomText1Name { get; set; } = string.Empty;
        public bool CustomText2State { get; set; }
        [StringLength(100)] public string CustomText2Name { get; set; } = string.Empty;
        public bool CustomText3State { get; set; }
        [StringLength(100)] public string CustomText3Name { get; set; } = string.Empty;

        // Numeric fields
        public bool CustomNumeric1State { get; set; }
        [StringLength(100)] public string CustomNumeric1Name { get; set; } = string.Empty;
        public bool CustomNumeric2State { get; set; }
        [StringLength(100)] public string CustomNumeric2Name { get; set; } = string.Empty;
        public bool CustomNumeric3State { get; set; }
        [StringLength(100)] public string CustomNumeric3Name { get; set; } = string.Empty;

        // Boolean (checkbox) fields
        public bool CustomBool1State { get; set; }
        [StringLength(100)] public string CustomBool1Name { get; set; } = string.Empty;
        public bool CustomBool2State { get; set; }
        [StringLength(100)] public string CustomBool2Name { get; set; } = string.Empty;
        public bool CustomBool3State { get; set; }
        [StringLength(100)] public string CustomBool3Name { get; set; } = string.Empty;

        // Document/Image Link fields
        public bool CustomLink1State { get; set; }
        [StringLength(100)] public string CustomLink1Name { get; set; } = string.Empty;
        public bool CustomLink2State { get; set; }
        [StringLength(100)] public string CustomLink2Name { get; set; } = string.Empty;
        public bool CustomLink3State { get; set; }
        [StringLength(100)] public string CustomLink3Name { get; set; } = string.Empty;
    }
}