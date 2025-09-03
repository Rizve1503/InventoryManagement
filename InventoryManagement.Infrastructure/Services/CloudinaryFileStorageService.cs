using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InventoryManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Services
{
    public class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorageService(IConfiguration configuration)
        {
            // Read credentials from configuration (appsettings.json and User Secrets)
            var account = new Account(
                // These calls to .GetValue<string>() will now work correctly.
                configuration.GetValue<string>("Cloudinary:CloudName"),
                configuration.GetValue<string>("Cloudinary:ApiKey"),
                configuration.GetValue<string>("Cloudinary:ApiSecret")
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty", nameof(file));
            }

            // Upload the file to Cloudinary
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                // Optional: Store images in a specific folder in Cloudinary
                Folder = "inventra_uploads"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            // Return the secure URL of the uploaded image
            return uploadResult.SecureUrl.ToString();
        }

        public void DeleteFile(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Cloudinary requires a "Public ID" to delete an image, not the full URL.
            // We need to extract this ID from the URL.
            // Example URL: https://res.cloudinary.com/cloud-name/image/upload/v12345/folder/public_id.jpg
            // We need to get "folder/public_id"

            try
            {
                var uri = new Uri(imageUrl);
                // Get the last part of the URL path and remove the file extension
                var publicIdWithFolder = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                // The full public ID includes the folder name
                var folder = "inventra_uploads";
                var publicId = $"{folder}/{publicIdWithFolder}";

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                // Asynchronously delete the image. We don't wait for the result in this case,
                // as it's a fire-and-forget operation from the user's perspective.
                _cloudinary.DestroyAsync(deletionParams);
            }
            catch (Exception)
            {
                // Log the error in a real application, but don't block the user.
                // It's possible the URL was invalid or the file was already deleted.
            }
        }
    }
}