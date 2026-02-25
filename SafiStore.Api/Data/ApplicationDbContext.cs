using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /* =========================
           DbSets
           ========================= */
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* =======================================================
               DECIMAL PRECISION
               ======================================================= */
            modelBuilder.Entity<Product>(entity =>
            {
                // Provider-agnostic precision
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.Rating).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(ci => ci.PriceAtAddition).HasPrecision(18, 2);
            });

            /* =========================
               CATEGORY ↔ PRODUCT
               ========================= */
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasMany(c => c.Products)
                      .WithOne(p => p.Category)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
               USER
               ========================= */
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasMany(u => u.Orders)
                      .WithOne(o => o.User)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Reviews)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* =========================
               CART (User ↔ Cart ↔ CartItems)
               ========================= */
            modelBuilder.Entity<Cart>(entity =>
            {
                // User ↔ Cart (One-to-One)
                entity.HasOne(c => c.User)
                      .WithOne()
                      .HasForeignKey<Cart>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Cart ↔ CartItems (One-to-Many)
                entity.HasMany(c => c.Items)
                      .WithOne(ci => ci.Cart)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
               ORDER ↔ ORDER ITEMS
               ========================= */
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasMany(o => o.Items)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* =========================
               PRODUCT ↔ REVIEWS
               ========================= */
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasMany(p => p.Reviews)
                      .WithOne(r => r.Product)
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* =========================
               SEED DATA
               ========================= */
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Books" },
                new Category { Id = 4, Name = "Home & Garden" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Laptop Pro",
                    Description = "High-performance laptop for professionals",
                    Price = 1299.99m,
                    Stock = 50,
                    CategoryId = 1,
                    ImageUrl = "https://via.placeholder.com/300",
                    Rating = 4.5m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Title = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse",
                    Price = 29.99m,
                    Stock = 200,
                    CategoryId = 1,
                    ImageUrl = "https://via.placeholder.com/300",
                    Rating = 4.0m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
