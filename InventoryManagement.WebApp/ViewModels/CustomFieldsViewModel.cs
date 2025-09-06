using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebApp.ViewModels
{
    public class CustomFieldsViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;
        public byte[]? RowVersion { get; set; }


        // Single-line text fields
        public bool CustomString1State { get; set; }
        [StringLength(100)] public string? CustomString1Name { get; set; }
        public bool CustomString2State { get; set; }
        [StringLength(100)] public string? CustomString2Name { get; set; }
        public bool CustomString3State { get; set; }
        [StringLength(100)] public string? CustomString3Name { get; set; }

        // Multi-line text fields
        public bool CustomText1State { get; set; }
        [StringLength(100)] public string? CustomText1Name { get; set; }
        public bool CustomText2State { get; set; }
        [StringLength(100)] public string? CustomText2Name { get; set; }
        public bool CustomText3State { get; set; }
        [StringLength(100)] public string? CustomText3Name { get; set; }

        // Numeric fields
        public bool CustomNumeric1State { get; set; }
        [StringLength(100)] public string? CustomNumeric1Name { get; set; }
        public bool CustomNumeric2State { get; set; }
        [StringLength(100)] public string? CustomNumeric2Name { get; set; }
        public bool CustomNumeric3State { get; set; }
        [StringLength(100)] public string? CustomNumeric3Name { get; set; }

        // Boolean (checkbox) fields
        public bool CustomBool1State { get; set; }
        [StringLength(100)] public string? CustomBool1Name { get; set; }
        public bool CustomBool2State { get; set; }
        [StringLength(100)] public string? CustomBool2Name { get; set; }
        public bool CustomBool3State { get; set; }
        [StringLength(100)] public string? CustomBool3Name { get; set; }

        // Document/Image Link fields
        public bool CustomLink1State { get; set; }
        [StringLength(100)] public string? CustomLink1Name { get; set; }
        public bool CustomLink2State { get; set; }
        [StringLength(100)] public string? CustomLink2Name { get; set; }
        public bool CustomLink3State { get; set; }
        [StringLength(100)] public string? CustomLink3Name { get; set; }

        // Select from List fields
        public bool CustomSelect1State { get; set; }
        [StringLength(100)] public string? CustomSelect1Name { get; set; }
        [StringLength(1000)] public string? CustomSelect1Options { get; set; }
        public bool CustomSelect2State { get; set; }
        [StringLength(100)] public string? CustomSelect2Name { get; set; }
        [StringLength(1000)] public string? CustomSelect2Options { get; set; }
        public bool CustomSelect3State { get; set; }
        [StringLength(100)] public string? CustomSelect3Name { get; set; }
        [StringLength(1000)] public string? CustomSelect3Options { get; set; }
    }
}