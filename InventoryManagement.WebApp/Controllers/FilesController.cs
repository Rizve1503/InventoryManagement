using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure;
using InventoryManagement.WebApp.Controllers;
using InventoryManagement.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InventoryManagement.WebApp.Controllers
{
    // This controller serves files from our secure, non-web-root storage location.
    public class FilesController : Controller
    {
        private readonly string _storagePath;

        public FilesController(IConfiguration configuration)
        {
            _storagePath = configuration.GetValue<string>("FileStorage:Path")
                ?? throw new InvalidOperationException("FileStorage:Path is not configured.");
        }

        // GET: /Files/GetImage/{fileName}
        [HttpGet]
        public IActionResult GetImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_storagePath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Return the file stream. The browser will handle rendering it as an image.
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "image/jpeg"); // Adjust content type as needed
        }
    }
}