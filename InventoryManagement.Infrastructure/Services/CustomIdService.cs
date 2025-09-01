using InventoryManagement.Application.Models.CustomId;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Services
{
    public class CustomIdService : ICustomIdService
    {
        private readonly ApplicationDbContext _context;

        public CustomIdService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateIdAsync(Inventory inventory)
        {
            return await GenerateIdInternalAsync(inventory, false);
        }

        public async Task<string> GeneratePreviewIdAsync(Inventory inventory)
        {
            return await GenerateIdInternalAsync(inventory, true);
        }

        private async Task<string> GenerateIdInternalAsync(Inventory inventory, bool isPreview)
        {
            if (string.IsNullOrEmpty(inventory.CustomIdFormatJson))
            {
                return string.Empty; // No format defined
            }

            var config = JsonSerializer.Deserialize<CustomIdConfiguration>(inventory.CustomIdFormatJson);
            if (config == null || !config.Elements.Any())
            {
                return string.Empty;
            }

            var idBuilder = new StringBuilder();
            var random = new Random();

            foreach (var element in config.Elements)
            {
                switch (element.Type)
                {
                    case CustomIdElementType.FixedText:
                        idBuilder.Append(element.Value);
                        break;

                    case CustomIdElementType.Sequence:
                        long sequence;
                        if (isPreview)
                        {
                            sequence = await _context.Items.CountAsync(i => i.InventoryId == inventory.Id) + 1;
                        }
                        else
                        {
                            // In a real high-concurrency scenario, this could have race conditions.
                            // A separate sequence table or more advanced locking would be needed.
                            // For this project's scope, this is sufficient.
                            sequence = await _context.Items.CountAsync(i => i.InventoryId == inventory.Id) + 1;
                        }
                        idBuilder.Append(sequence.ToString(element.Format ?? "D")); // e.g., "D5" for 00001
                        break;

                    case CustomIdElementType.RandomString:
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        int length = int.TryParse(element.Format, out int len) ? len : 6;
                        var randomString = new string(Enumerable.Repeat(chars, length)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        idBuilder.Append(randomString);
                        break;

                    case CustomIdElementType.DateTime:
                        idBuilder.Append(DateTime.UtcNow.ToString(element.Format ?? "yyyyMMdd"));
                        break;
                }
            }

            return idBuilder.ToString();
        }
    }
}