using Microsoft.EntityFrameworkCore;
using ODataProductsApi.Models;

namespace ODataProductsApi.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product configuration
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" },
                new Category { Id = 3, Name = "Clothing" }
            );

            // Seed products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-performance gaming laptop", Price = 1200.00m, CategoryId = 1 },
                new Product { Id = 2, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 25.00m, CategoryId = 1 },
                new Product { Id = 3, Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard", Price = 80.00m, CategoryId = 1 },
                new Product { Id = 4, Name = "USB-C Cable", Description = "2m USB-C charging cable", Price = 12.00m, CategoryId = 1 },
                new Product { Id = 5, Name = "C# Programming Book", Description = "Learn C# in depth", Price = 45.00m, CategoryId = 2 },
                new Product { Id = 6, Name = "Design Patterns", Description = "Software design patterns guide", Price = 55.00m, CategoryId = 2 },
                new Product { Id = 7, Name = "Cotton T-Shirt", Description = "Comfortable cotton t-shirt", Price = 15.00m, CategoryId = 3 },
                new Product { Id = 8, Name = "Jeans", Description = "Blue denim jeans", Price = 60.00m, CategoryId = 3 },
                new Product { Id = 9, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 150.00m, CategoryId = 1 },
                new Product { Id = 10, Name = "Winter Jacket", Description = "Warm winter jacket", Price = 120.00m, CategoryId = 3 }
            );
        }
    }