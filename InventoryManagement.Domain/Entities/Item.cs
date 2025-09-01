using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string? CustomId { get; set; } // The user-defined, formatted ID
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // --- Relationships ---
        public int InventoryId { get; set; }
        public virtual Inventory? Inventory { get; set; }

        // --- Concurrency ---
        public byte[]? RowVersion { get; set; }

        // --- Custom Field Values (Fixed Schema) ---
        // These properties hold the actual data entered by the user for an item.

        // Single-line text values
        public string? CustomString1Value { get; set; }
        public string? CustomString2Value { get; set; }
        public string? CustomString3Value { get; set; }

        // Multi-line text values
        public string? CustomText1Value { get; set; }
        public string? CustomText2Value { get; set; }
        public string? CustomText3Value { get; set; }

        // Numeric values
        public decimal? CustomNumeric1Value { get; set; }
        public decimal? CustomNumeric2Value { get; set; }
        public decimal? CustomNumeric3Value { get; set; }

        // Boolean (checkbox) values
        public bool? CustomBool1Value { get; set; }
        public bool? CustomBool2Value { get; set; }
        public bool? CustomBool3Value { get; set; }

        // Document/Image Link values
        public string? CustomLink1Value { get; set; }
        public string? CustomLink2Value { get; set; }
        public string? CustomLink3Value { get; set; }

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    }
}

