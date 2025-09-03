using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        void DeleteFile(string fileName);
    }
}