using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("Database already seeded.");
                return;
            }

            logger.LogInformation("Seeding initial data...");

            // Categories matching Frontend filter options
            var categories = new Dictionary<string, Category>
            {
                ["Computers"] = new Category { Name = "Computers" },
                ["Wearables"] = new Category { Name = "Wearables" },
                ["Phone"]     = new Category { Name = "Phone" },
                ["Audio"]     = new Category { Name = "Audio" },
                ["Gaming"]    = new Category { Name = "Gaming" },
                ["Accessories"] = new Category { Name = "Accessories" }
            };

            await context.Categories.AddRangeAsync(categories.Values);
            await context.SaveChangesAsync();

            // Reload to get IDs assigned
            foreach (var kvp in categories.ToList())
            {
                var saved = await context.Categories.FirstAsync(c => c.Name == kvp.Key);
                categories[kvp.Key] = saved;
            }

            var now = DateTime.UtcNow;

            var products = new List<Product>
            {
                // ── Laptops (Computers) ──
                new()
                {
                    Title = "MacBook Pro 16\" M3 Max",
                    Description = "Supercharged by M3 Max with up to 128GB unified memory. Stunning 16.2-inch Liquid Retina XDR display with extreme dynamic range and 1000 nits brightness. Up to 22 hours of battery life.",
                    Price = 3499.00m, Stock = 25,
                    CategoryId = categories["Computers"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800",
                    ComparePrice = 3799.00m,
                    Rating = 4.9m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "Dell XPS 15 OLED",
                    Description = "Intel Core i9-13900H, 32GB DDR5, 1TB SSD, NVIDIA GeForce RTX 4070. 15.6-inch 3.5K OLED InfinityEdge display with 100% DCI-P3 color gamut.",
                    Price = 2499.99m, Stock = 18,
                    CategoryId = categories["Computers"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=800",
                    Rating = 4.7m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "ASUS ROG Zephyrus G14",
                    Description = "AMD Ryzen 9 7940HS, 16GB DDR5, RTX 4060, 14-inch QHD 165Hz display. Ultra-portable gaming powerhouse with premium build quality.",
                    Price = 1799.99m, Stock = 35,
                    CategoryId = categories["Computers"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b5f0a8a?w=800",
                    ComparePrice = 1999.99m,
                    Rating = 4.6m,
                    CreatedAt = now, UpdatedAt = now
                },
                // ── Phones (Phone) ──
                new()
                {
                    Title = "iPhone 15 Pro Max",
                    Description = "A17 Pro chip, 48MP main camera with 5x optical zoom, titanium design, 256GB storage. The ultimate iPhone with USB-C 3.0. 8K video recording at 30fps.",
                    Price = 1199.00m, Stock = 40,
                    CategoryId = categories["Phone"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a735a44f0b7?w=800",
                    ComparePrice = 1399.00m,
                    Rating = 4.8m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "Samsung Galaxy S24 Ultra",
                    Description = "Snapdragon 8 Gen 3, 200MP camera with AI editing, S Pen built-in, Galaxy AI, 512GB. AI-powered epic experience with 7 years of updates. Titanium frame.",
                    Price = 1299.99m, Stock = 30,
                    CategoryId = categories["Phone"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1705491197419-1b3e98f82d73?w=800",
                    Rating = 4.6m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "Google Pixel 8 Pro",
                    Description = "Tensor G3, 50MP main camera with Magic Editor, 7 years of updates, 128GB. The smartest Pixel yet with advanced AI photo tools.",
                    Price = 999.00m, Stock = 45,
                    CategoryId = categories["Phone"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1696426057727-4e0d6d5d0b8c?w=800",
                    ComparePrice = 1099.00m,
                    Rating = 4.5m,
                    CreatedAt = now, UpdatedAt = now
                },
                // ── Headphones (Audio) ──
                new()
                {
                    Title = "Sony WH-1000XM5",
                    Description = "Industry-leading noise cancellation with Auto NC Optimizer. 30-hour battery life, crystal-clear hands-free calling, and exceptionally comfortable lightweight design.",
                    Price = 349.99m, Stock = 60,
                    CategoryId = categories["Audio"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?w=800",
                    ComparePrice = 399.99m,
                    Rating = 4.7m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "Apple AirPods Pro 2",
                    Description = "Up to 2x more Active Noise Cancellation, Adaptive Transparency, personalized spatial audio. USB-C charging case with Find My support.",
                    Price = 249.00m, Stock = 80,
                    CategoryId = categories["Audio"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1606220945775-6a7831d5f4f3?w=800",
                    ComparePrice = 299.00m,
                    Rating = 4.8m,
                    CreatedAt = now, UpdatedAt = now
                },
                // ── Watches (Wearables) ──
                new()
                {
                    Title = "Apple Watch Ultra 2",
                    Description = "49mm titanium case, precision dual-frequency GPS, action button, up to 36 hours battery life. Precision dual-frequency GPS. Extreme sports companion with diving computer.",
                    Price = 799.00m, Stock = 50,
                    CategoryId = categories["Wearables"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1694566382298-205f3e1f8e14?w=800",
                    Rating = 4.8m,
                    CreatedAt = now, UpdatedAt = now
                },
                new()
                {
                    Title = "Samsung Galaxy Watch 6 Classic",
                    Description = "47mm, rotating bezel, BioActive sensor, body composition analysis, Wear OS. Classic design meets modern health tracking with sapphire crystal display.",
                    Price = 449.99m, Stock = 60,
                    CategoryId = categories["Wearables"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1694999624021-5c0c4f3f0a9e?w=800",
                    ComparePrice = 499.99m,
                    Rating = 4.5m,
                    CreatedAt = now, UpdatedAt = now
                },
                // ── Gaming ──
                new()
                {
                    Title = "PlayStation 5 Slim",
                    Description = "Ultra-fast SSD, 4K gaming, ray tracing, 825GB SSD. The best gaming experience with haptic feedback, adaptive triggers, and Tempest 3D AudioTech.",
                    Price = 499.99m, Stock = 20,
                    CategoryId = categories["Gaming"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=800",
                    Rating = 4.9m,
                    CreatedAt = now, UpdatedAt = now
                },
                // ── Accessories ──
                new()
                {
                    Title = "Logitech MX Master 3S",
                    Description = "Ergonomic wireless mouse with 8K DPI optical sensor, quiet clicks, USB-C, MagSpeed scroll wheel. 70-day battery life. Perfect for productivity.",
                    Price = 99.99m, Stock = 100,
                    CategoryId = categories["Accessories"].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1629429408209-1f912961dbd8?w=800",
                    ComparePrice = 129.99m,
                    Rating = 4.6m,
                    CreatedAt = now, UpdatedAt = now
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeding completed: {Count} categories, {ProductCount} products.",
                categories.Count, products.Count);
        }
    }
}
