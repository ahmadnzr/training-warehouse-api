using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockMovementItem> StockMovementItems => Set<StockMovementItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureWarehouses(modelBuilder);
        ConfigureCategories(modelBuilder);
        ConfigureProductCategories(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureWarehouseLocations(modelBuilder);
        ConfigureSuppliers(modelBuilder);
        ConfigureStockLevels(modelBuilder);
        ConfigureStockMovements(modelBuilder);
        ConfigureStockMovementItems(modelBuilder);

    }


    private static void ConfigureStockLevels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockLevel>(entity =>
        {
            entity.ToTable("stock_levels");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantity).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.ProductId, e.WarehouseLocationId }).IsUnique();
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.WarehouseLocationId);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.WarehouseLocation)
                  .WithMany()
                  .HasForeignKey(e => e.WarehouseLocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureStockMovements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.ToTable("stock_movements");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.MovementNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.Property(e => e.Type)
                  .HasConversion(
                      v => v.ToString().ToLowerInvariant(),
                      v => Enum.Parse<StockMovementType>(v, true))
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasConversion(
                      v => v.ToString().ToLowerInvariant(),
                      v => Enum.Parse<StockMovementStatus>(v, true))
                  .HasMaxLength(50)
                  .IsRequired();

            entity.HasIndex(e => e.MovementNumber).IsUnique();
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.SupplierId);
            entity.HasIndex(e => e.CreatedByUserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.DeletedAt);

            entity.HasOne(e => e.Supplier)
                  .WithMany()
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureStockMovementItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockMovementItem>(entity =>
        {
            entity.ToTable("stock_movement_items");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantity).IsRequired();

            entity.HasIndex(e => e.StockMovementId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.SourceLocationId);
            entity.HasIndex(e => e.DestinationLocationId);

            entity.HasOne(e => e.StockMovement)
                  .WithMany(m => m.Items)
                  .HasForeignKey(e => e.StockMovementId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SourceLocation)
                  .WithMany()
                  .HasForeignKey(e => e.SourceLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DestinationLocation)
                  .WithMany()
                  .HasForeignKey(e => e.DestinationLocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSuppliers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(500);

            entity.HasIndex(e => e.Code)
                  .IsUnique()
                  .HasFilter("[DeletedAt] IS NULL");

            entity.HasIndex(e => e.UserId)
                  .IsUnique()
                  .HasFilter("[UserId] IS NOT NULL AND [DeletedAt] IS NULL");
        });
    }


    private static void ConfigureCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(category => category.Id);
            entity.HasIndex(category => category.Name);
            entity.HasIndex(category => category.DeletedAt);

            entity.Property(category => category.Name).HasMaxLength(150).IsRequired();
            entity.Property(category => category.IsActive).HasDefaultValue(true);
            entity.Property(category => category.CreatedAt).IsRequired();
        });
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasIndex(user => user.Role);
            entity.HasIndex(user => user.DeletedAt);

            entity.Property(user => user.Name).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(255).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(50);
            entity.Property(user => user.IsActive).HasDefaultValue(false);
            entity.Property(user => user.CreatedAt).IsRequired();
        });
    }

    private static void ConfigureWarehouses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouses");

            entity.HasKey(warehouse => warehouse.Id);
            entity.HasIndex(warehouse => warehouse.Code).IsUnique();
            entity.HasIndex(warehouse => warehouse.Name);
            entity.HasIndex(warehouse => warehouse.DeletedAt);

            entity.Property(warehouse => warehouse.Code).HasMaxLength(50).IsRequired();
            entity.Property(warehouse => warehouse.Name).HasMaxLength(150).IsRequired();
            entity.Property(warehouse => warehouse.Address).HasMaxLength(500);
            entity.Property(warehouse => warehouse.City).HasMaxLength(100);
            entity.Property(warehouse => warehouse.IsActive).HasDefaultValue(true);
            entity.Property(warehouse => warehouse.CreatedAt).IsRequired();
        });
    }

    private static void ConfigureWarehouseLocations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WarehouseLocation>(entity =>
        {
            entity.ToTable("warehouse_locations");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.WarehouseId, e.Code })
                  .IsUnique()
                  .HasFilter("[DeletedAt] IS NULL");

            entity.HasOne(wl => wl.Warehouse)
                  .WithMany(w => w.Locations)
                  .HasForeignKey(wl => wl.WarehouseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureProductCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("product_categories");

            // Composite Primary Key
            entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            entity.HasIndex(pc => pc.ProductId);
            entity.HasIndex(pc => pc.CategoryId);

            entity.Property(pc => pc.CreatedAt).IsRequired();

            entity.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(product => product.Id);
            entity.HasIndex(product => product.Sku).IsUnique().HasFilter("[DeletedAt] IS NULL");
            entity.HasIndex(product => product.Name);
            entity.HasIndex(product => product.DeletedAt);

            entity.Property(product => product.Sku).HasMaxLength(100).IsRequired();
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Unit).HasMaxLength(50).IsRequired();
            entity.Property(product => product.Weight).HasPrecision(18, 2);
            entity.Property(product => product.IsActive).HasDefaultValue(true);
            entity.Property(product => product.CreatedAt).IsRequired();
        });
    }


}
