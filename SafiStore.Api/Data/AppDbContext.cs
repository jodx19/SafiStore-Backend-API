using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /* =========================
           DbSets
           ========================= */
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize Identity Table Names & Mapping to match existing DB (Users table)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                // Explicit column mappings to match existing schema
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("Id");

                entity.Property(u => u.FirstName)
                      .HasColumnName("FirstName")
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                      .HasColumnName("LastName")
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .HasColumnName("Email")
                      .HasMaxLength(450);

                entity.Property(u => u.UserName)
                      .HasColumnName("UserName")
                      .HasMaxLength(256);

                entity.Property(u => u.NormalizedEmail)
                      .HasColumnName("NormalizedEmail")
                      .HasMaxLength(256);

                entity.Property(u => u.NormalizedUserName)
                      .HasColumnName("NormalizedUserName")
                      .HasMaxLength(256);

                entity.Property(u => u.PasswordHash)
                      .HasColumnName("PasswordHash");

                entity.Property(u => u.PhoneNumber)
                      .HasColumnName("PhoneNumber");

                entity.Property(u => u.Role)
                      .HasColumnName("Role")
                      .HasMaxLength(50);

                entity.Property(u => u.Address)
                      .HasColumnName("Address")
                      .HasMaxLength(255);

                entity.Property(u => u.City)
                      .HasColumnName("City")
                      .HasMaxLength(50);

                entity.Property(u => u.Country)
                      .HasColumnName("Country")
                      .HasMaxLength(50);

                entity.Property(u => u.PostalCode)
                      .HasColumnName("PostalCode")
                      .HasMaxLength(10);

                entity.Property(u => u.IsActive)
                      .HasColumnName("IsActive");

                entity.Property(u => u.CreatedAt)
                      .HasColumnName("CreatedAt");

                entity.Property(u => u.UpdatedAt)
                      .HasColumnName("UpdatedAt");

                // Refresh token fields
                entity.Property(u => u.RefreshToken)
                      .HasColumnName("RefreshToken");

                entity.Property(u => u.RefreshTokenExpiry)
                      .HasColumnName("RefreshTokenExpiry");

                // Concurrency/identity stamps (map if present)
                entity.Property<string>("ConcurrencyStamp").HasColumnName("ConcurrencyStamp");
                entity.Property<string>("SecurityStamp").HasColumnName("SecurityStamp");

                // Ignore local-only properties that don't exist in SQL
                entity.Ignore(u => u.AvatarUrl);
            });

            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

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
               USER Relationships
               ========================= */
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.Orders)
                      .WithOne(o => o.User)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Reviews)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
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
        }
    }
}
