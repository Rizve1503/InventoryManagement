using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebApp.ViewModels
{
    public class InventoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Make Public?")]
        public bool IsPublic { get; set; }

        // For optimistic concurrency
        public byte[]? RowVersion { get; set; }

        [Display(Name = "Tags (comma-separated)")]
        public string? Tags { get; set; }


        [Display(Name = "Inventory Image")]
        public IFormFile? ImageFile { get; set; } // For the file upload
        public string? ExistingImageUrl { get; set; } // To display the current image
    }
}