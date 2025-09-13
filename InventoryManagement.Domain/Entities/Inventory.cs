using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // --- Relationships ---
        public int CreatorId { get; set; }
        public virtual User? Creator { get; set; }
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

        // --- Concurrency ---
        public byte[]? RowVersion { get; set; }

        // --- Custom Field Definitions (Fixed Schema) ---
        // Determines if a field is active and what its display name is.

        // Single-line text fields
        public bool CustomString1State { get; set; }
        public string CustomString1Name { get; set; } = string.Empty;
        public bool CustomString2State { get; set; }
        public string CustomString2Name { get; set; } = string.Empty;
        public bool CustomString3State { get; set; }
        public string CustomString3Name { get; set; } = string.Empty;

        // Multi-line text fields
        public bool CustomText1State { get; set; }
        public string CustomText1Name { get; set; } = string.Empty;
        public bool CustomText2State { get; set; }
        public string CustomText2Name { get; set; } = string.Empty;
        public bool CustomText3State { get; set; }
        public string CustomText3Name { get; set; } = string.Empty;

        // Numeric fields
        public bool CustomNumeric1State { get; set; }
        public string CustomNumeric1Name { get; set; } = string.Empty;
        public bool CustomNumeric2State { get; set; }
        public string CustomNumeric2Name { get; set; } = string.Empty;
        public bool CustomNumeric3State { get; set; }
        public string CustomNumeric3Name { get; set; } = string.Empty;

        // Boolean (checkbox) fields
        public bool CustomBool1State { get; set; }
        public string CustomBool1Name { get; set; } = string.Empty;
        public bool CustomBool2State { get; set; }
        public string CustomBool2Name { get; set; } = string.Empty;
        public bool CustomBool3State { get; set; }
        public string CustomBool3Name { get; set; } = string.Empty;

        // Document/Image Link fields
        public bool CustomLink1State { get; set; }
        public string CustomLink1Name { get; set; } = string.Empty;
        public bool CustomLink2State { get; set; }
        public string CustomLink2Name { get; set; } = string.Empty;
        public bool CustomLink3State { get; set; }
        public string CustomLink3Name { get; set; } = string.Empty;
        public string? CustomIdFormatJson { get; set; }

        // Select from List fields
        public bool CustomSelect1State { get; set; }
        public string CustomSelect1Name { get; set; } = string.Empty;
        public string CustomSelect1Options { get; set; } = string.Empty; // Comma-separated options

        public bool CustomSelect2State { get; set; }
        public string CustomSelect2Name { get; set; } = string.Empty;
        public string CustomSelect2Options { get; set; } = string.Empty;

        public bool CustomSelect3State { get; set; }
        public string CustomSelect3Name { get; set; } = string.Empty;
        public string CustomSelect3Options { get; set; } = string.Empty;

        // --- Validation Rules for String Fields ---
        public int? CustomString1MaxLength { get; set; }
        public string? CustomString1Regex { get; set; } // For regex pattern validation
        public int? CustomString2MaxLength { get; set; }
        public string? CustomString2Regex { get; set; }
        public int? CustomString3MaxLength { get; set; }
        public string? CustomString3Regex { get; set; }

        // --- Validation Rules for Numeric Fields ---
        public decimal? CustomNumeric1MinValue { get; set; }
        public decimal? CustomNumeric1MaxValue { get; set; }
        public decimal? CustomNumeric2MinValue { get; set; }
        public decimal? CustomNumeric2MaxValue { get; set; }
        public decimal? CustomNumeric3MinValue { get; set; }
        public decimal? CustomNumeric3MaxValue { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public string? CustomFieldOrderJson { get; set; }

    }
}
