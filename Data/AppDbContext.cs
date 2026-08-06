using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockMovementItem> StockMovementItems => Set<StockMovementItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureWarehouses(modelBuilder);
        ConfigureWarehouseLocations(modelBuilder);
        ConfigureProductCategories(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureSuppliers(modelBuilder);
        ConfigureStockLevels(modelBuilder);
        ConfigureStockMovements(modelBuilder);
        ConfigureStockMovementItems(modelBuilder);
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

            entity.HasKey(location => location.Id);
            entity.HasIndex(location => new { location.WarehouseId, location.Code }).IsUnique();
            entity.HasIndex(location => location.WarehouseId);
            entity.HasIndex(location => location.DeletedAt);

            entity.Property(location => location.Code).HasMaxLength(50).IsRequired();
            entity.Property(location => location.Name).HasMaxLength(150).IsRequired();
            entity.Property(location => location.IsActive).HasDefaultValue(true);
            entity.Property(location => location.CreatedAt).IsRequired();

            entity.HasOne(location => location.Warehouse)
                .WithMany(warehouse => warehouse.Locations)
                .HasForeignKey(location => location.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureProductCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("product_categories");

            entity.HasKey(category => category.Id);
            entity.HasIndex(category => category.Name);
            entity.HasIndex(category => category.DeletedAt);

            entity.Property(category => category.Name).HasMaxLength(150).IsRequired();
            entity.Property(category => category.IsActive).HasDefaultValue(true);
            entity.Property(category => category.CreatedAt).IsRequired();
        });
    }

    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(product => product.Id);
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasIndex(product => product.Name);
            entity.HasIndex(product => product.ProductCategoryId);
            entity.HasIndex(product => product.DeletedAt);

            entity.Property(product => product.Sku).HasMaxLength(100).IsRequired();
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Unit).HasMaxLength(50).IsRequired();
            entity.Property(product => product.Weight).HasPrecision(18, 2);
            entity.Property(product => product.IsActive).HasDefaultValue(true);
            entity.Property(product => product.CreatedAt).IsRequired();

            entity.HasOne(product => product.ProductCategory)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSuppliers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");

            entity.HasKey(supplier => supplier.Id);
            entity.HasIndex(supplier => supplier.Name);
            entity.HasIndex(supplier => supplier.Email);
            entity.HasIndex(supplier => supplier.DeletedAt);

            entity.Property(supplier => supplier.Name).HasMaxLength(150).IsRequired();
            entity.Property(supplier => supplier.Phone).HasMaxLength(50);
            entity.Property(supplier => supplier.Email).HasMaxLength(255);
            entity.Property(supplier => supplier.Address).HasMaxLength(500);
            entity.Property(supplier => supplier.IsActive).HasDefaultValue(true);
            entity.Property(supplier => supplier.CreatedAt).IsRequired();
        });
    }

    private static void ConfigureStockLevels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockLevel>(entity =>
        {
            entity.ToTable("stock_levels");

            entity.HasKey(stockLevel => stockLevel.Id);
            entity.HasIndex(stockLevel => new { stockLevel.ProductId, stockLevel.WarehouseLocationId }).IsUnique();
            entity.HasIndex(stockLevel => stockLevel.ProductId);
            entity.HasIndex(stockLevel => stockLevel.WarehouseLocationId);

            entity.Property(stockLevel => stockLevel.Quantity).HasDefaultValue(0);
            entity.Property(stockLevel => stockLevel.CreatedAt).IsRequired();

            entity.HasOne(stockLevel => stockLevel.Product)
                .WithMany(product => product.StockLevels)
                .HasForeignKey(stockLevel => stockLevel.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(stockLevel => stockLevel.WarehouseLocation)
                .WithMany(location => location.StockLevels)
                .HasForeignKey(stockLevel => stockLevel.WarehouseLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureStockMovements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.ToTable("stock_movements");

            entity.HasKey(movement => movement.Id);
            entity.HasIndex(movement => movement.MovementNumber).IsUnique();
            entity.HasIndex(movement => movement.Type);
            entity.HasIndex(movement => movement.Status);
            entity.HasIndex(movement => movement.SupplierId);
            entity.HasIndex(movement => movement.CreatedByUserId);
            entity.HasIndex(movement => movement.CreatedAt);
            entity.HasIndex(movement => movement.DeletedAt);

            entity.Property(movement => movement.MovementNumber).HasMaxLength(100).IsRequired();
            entity.Property(movement => movement.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(movement => movement.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(movement => movement.Notes).HasMaxLength(1000);
            entity.Property(movement => movement.CreatedAt).IsRequired();

            entity.HasOne(movement => movement.Supplier)
                .WithMany(supplier => supplier.StockMovements)
                .HasForeignKey(movement => movement.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(movement => movement.CreatedByUser)
                .WithMany(user => user.StockMovements)
                .HasForeignKey(movement => movement.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureStockMovementItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockMovementItem>(entity =>
        {
            entity.ToTable("stock_movement_items");

            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.StockMovementId);
            entity.HasIndex(item => item.ProductId);
            entity.HasIndex(item => item.SourceLocationId);
            entity.HasIndex(item => item.DestinationLocationId);

            entity.Property(item => item.Quantity).IsRequired();

            entity.HasOne(item => item.StockMovement)
                .WithMany(movement => movement.Items)
                .HasForeignKey(item => item.StockMovementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.Product)
                .WithMany(product => product.StockMovementItems)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.SourceLocation)
                .WithMany(location => location.SourceMovementItems)
                .HasForeignKey(item => item.SourceLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.DestinationLocation)
                .WithMany(location => location.DestinationMovementItems)
                .HasForeignKey(item => item.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
