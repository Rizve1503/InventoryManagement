using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.WebApp.ViewModels
{
    public class CommentViewModel
    {
        public int InventoryId { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
    }
}