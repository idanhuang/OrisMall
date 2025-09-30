using Microsoft.EntityFrameworkCore;
using OrisMall.Core.Entities;

namespace OrisMall.Infrastructure.Data;

public class OrisMallDbContext : DbContext
{
    public OrisMallDbContext(DbContextOptions<OrisMallDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SKU).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Configure relationship with Category
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = 3, Name = "Books", Description = "Books and literature", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = 4, Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Smartphone", Description = "Latest generation smartphone with advanced features", Price = 699.99m, StockQuantity = 50, SKU = "PHONE001", IsActive = true, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Laptop", Description = "High-performance laptop for work and gaming", Price = 1299.99m, StockQuantity = 25, SKU = "LAPTOP001", IsActive = true, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "T-Shirt", Description = "Comfortable cotton t-shirt", Price = 19.99m, StockQuantity = 100, SKU = "TSHIRT001", IsActive = true, CategoryId = 2, CreatedAt = DateTime.UtcNow },
            new Product { Id = 4, Name = "Programming Book", Description = "Comprehensive guide to modern programming", Price = 49.99m, StockQuantity = 75, SKU = "BOOK001", IsActive = true, CategoryId = 3, CreatedAt = DateTime.UtcNow }
        );
    }
}
