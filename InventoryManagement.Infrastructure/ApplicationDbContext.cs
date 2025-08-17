using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // We will add DbSet properties for our entities here in future milestones.
    }
}