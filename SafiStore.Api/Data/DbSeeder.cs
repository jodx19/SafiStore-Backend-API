using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            // Basic idempotent seeding: only add defaults when none exist
            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("Database already seeded.");
                return;
            }

            logger.LogInformation("Seeding initial data...");

            var categories = new[] {
                new Category { Name = "Electronics" },
                new Category { Name = "Clothing" },
                new Category { Name = "Books" },
                new Category { Name = "Home & Garden" }
            };

            await context.Categories.AddRangeAsync(categories);

            // Identity users should be seeded via UserManager
            // Demo users creation removed.

            var laptop = new Product
            {
                Title = "Laptop Pro",
                Description = "High-performance laptop for professionals",
                Price = 1299.99m,
                Stock = 50,
                Category = categories[0],
                ImageUrl = "https://via.placeholder.com/300",
                Rating = 4.5m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var mouse = new Product
            {
                Title = "Wireless Mouse",
                Description = "Ergonomic wireless mouse",
                Price = 29.99m,
                Stock = 200,
                Category = categories[0],
                ImageUrl = "https://via.placeholder.com/300",
                Rating = 4.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Products.AddRangeAsync(laptop, mouse);

            await context.SaveChangesAsync();

            logger.LogInformation("Seeding completed.");
        }
    }
}
