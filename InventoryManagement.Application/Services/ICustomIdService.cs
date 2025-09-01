using InventoryManagement.Domain.Entities;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Services
{
    public interface ICustomIdService
    {
        Task<string> GenerateIdAsync(Inventory inventory);
        Task<string> GeneratePreviewIdAsync(Inventory inventory);
    }
}