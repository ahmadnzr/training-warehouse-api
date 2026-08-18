using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Helpers;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await SeedUsersAsync(dbContext);
        await SeedWarehousesAsync(dbContext);
        await SeedWarehouseLocationsAsync(dbContext);
        await SeedCategoriesAsync(dbContext);
        await SeedProductsAsync(dbContext);
        await SeedSuppliersAsync(dbContext);
        await SeedStockLevelsAsync(dbContext);
        await SeedStockMovementsAsync(dbContext);
    }


    private static async Task SeedProductsAsync(AppDbContext dbContext)
    {
        if (await dbContext.Products.AnyAsync(p => p.DeletedAt == null)) return;

        var now = DateTime.UtcNow;

        var makananCat = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Makanan & Minuman");
        var elektronikCat = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Elektronik");

        var products = new[]
        {
            new Product { Id = Guid.NewGuid(), Sku = "PRD-001", Name = "Indomie Goreng", Unit = "Pcs", Weight = 0.08m, IsActive = true, CreatedAt = now },
            new Product { Id = Guid.NewGuid(), Sku = "PRD-002", Name = "Kopi Kenangan Mantan", Unit = "Cup", Weight = 0.25m, IsActive = true, CreatedAt = now },
            new Product { Id = Guid.NewGuid(), Sku = "PRD-003", Name = "Laptop ASUS ROG", Unit = "Unit", Weight = 2.50m, IsActive = true, CreatedAt = now }
        };

        foreach (var product in products)
        {
            dbContext.Products.Add(product);

            if ((product.Sku == "PRD-001" || product.Sku == "PRD-002") && makananCat != null)
            {
                dbContext.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = makananCat.Id,
                    CreatedAt = now
                });
            }
            else if (product.Sku == "PRD-003" && elektronikCat != null)
            {
                dbContext.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = elektronikCat.Id,
                    CreatedAt = now
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(AppDbContext dbContext)
    {
        if (await dbContext.Categories.AnyAsync(c => c.DeletedAt == null))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var categories = new[]
        {
        new Category { Name = "Makanan & Minuman", IsActive = true, CreatedAt = now },
        new Category { Name = "Elektronik", IsActive = true, CreatedAt = now },
        new Category { Name = "Pakaian & Tekstil", IsActive = true, CreatedAt = now },
        new Category { Name = "Alat Tulis Kantor", IsActive = true, CreatedAt = now },
        new Category { Name = "Perlengkapan Rumah Tangga", IsActive = true, CreatedAt = now }
    };

        foreach (var category in categories)
        {
            var exists = await dbContext.Categories
                .AnyAsync(c => c.Name == category.Name && c.DeletedAt == null);

            if (!exists)
            {
                dbContext.Categories.Add(category);
            }
        }

        await dbContext.SaveChangesAsync();
    }
    private static async Task SeedWarehousesAsync(AppDbContext dbContext)
    {
        if (await dbContext.Warehouses.AnyAsync(w => w.DeletedAt == null))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var warehouses = new[]
        {
        new Warehouse { Code = "WH-JKT", Name = "Gudang Jakarta", Address = "Jl. Sudirman No. 1", City = "Jakarta", IsActive = true, CreatedAt = now },
        new Warehouse { Code = "WH-SBY", Name = "Gudang Surabaya", Address = "Jl. Ahmad Yani No. 10", City = "Surabaya", IsActive = true, CreatedAt = now },
        new Warehouse { Code = "WH-BDG", Name = "Gudang Bandung", Address = "Jl. Asia Afrika No. 5", City = "Bandung", IsActive = true, CreatedAt = now },
        new Warehouse { Code = "WH-MDN", Name = "Gudang Medan", Address = "Jl. Gatot Subroto No. 20", City = "Medan", IsActive = true, CreatedAt = now },
        new Warehouse { Code = "WH-MKS", Name = "Gudang Makassar", Address = "Jl. Pettarani No. 15", City = "Makassar", IsActive = true, CreatedAt = now },
    };

        foreach (var warehouse in warehouses)
        {
            var exists = await dbContext.Warehouses
                .AnyAsync(w => w.Code == warehouse.Code && w.DeletedAt == null);

            if (!exists)
            {
                dbContext.Warehouses.Add(warehouse);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var defaultPassword = "Admin123!";
        var passwordHash = PasswordHasher.HashPassword(defaultPassword);

        var seedUsers = new[]
        {
            new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Name = "Supervisor",
                Email = "supervisor@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.Supervisor,
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Name = "Warehouse Operator",
                Email = "operator@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.WarehouseOperator,
                IsActive = true,
                CreatedAt = now
            }
        };

        foreach (var seedUser in seedUsers)
        {
            var existing = await dbContext.Users
                .FirstOrDefaultAsync(user => user.Email == seedUser.Email);

            if (existing == null)
            {
                dbContext.Users.Add(seedUser);
            }
            else if (existing.PasswordHash == "TEMP_PASSWORD_HASH_CHANGE_WHEN_AUTH_IS_IMPLEMENTED")
            {
                existing.PasswordHash = passwordHash;
                existing.IsActive = true;
                existing.Role = seedUser.Role;
                existing.UpdatedAt = now;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedWarehouseLocationsAsync(AppDbContext dbContext)
    {
        if (await dbContext.WarehouseLocations.AnyAsync()) return;

        var warehouse = await dbContext.Warehouses.FirstOrDefaultAsync(w => w.Code == "WH-JKT");
        if (warehouse == null) return;

        var now = DateTime.UtcNow;
        var locations = new[]
        {
            new WarehouseLocation { Code = "A-01", Name = "Rak A-01", WarehouseId = warehouse.Id, IsActive = true, CreatedAt = now },
            new WarehouseLocation { Code = "A-02", Name = "Rak A-02", WarehouseId = warehouse.Id, IsActive = true, CreatedAt = now },
            new WarehouseLocation { Code = "B-01", Name = "Rak B-01", WarehouseId = warehouse.Id, IsActive = true, CreatedAt = now }
        };
        dbContext.WarehouseLocations.AddRange(locations);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedSuppliersAsync(AppDbContext dbContext)
    {
        if (await dbContext.Suppliers.AnyAsync()) return;

        var now = DateTime.UtcNow;
        var suppliers = new[]
        {
            new Supplier { Code = "SPL-001", Name = "PT Indofood Sukses Makmur", Phone = "021-123456", Email = "contact@indofood.com", Address = "Jakarta", CreatedAt = now },
            new Supplier { Code = "SPL-002", Name = "PT Kenangan Pasti", Phone = "021-654321", Email = "supply@kopikenangan.com", Address = "Jakarta", CreatedAt = now }
        };
        dbContext.Suppliers.AddRange(suppliers);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedStockLevelsAsync(AppDbContext dbContext)
    {
        if (await dbContext.StockLevels.AnyAsync()) return;

        var prd1 = await dbContext.Products.FirstOrDefaultAsync(p => p.Sku == "PRD-001");
        var prd2 = await dbContext.Products.FirstOrDefaultAsync(p => p.Sku == "PRD-002");
        var locA01 = await dbContext.WarehouseLocations.FirstOrDefaultAsync(l => l.Code == "A-01");
        var locB01 = await dbContext.WarehouseLocations.FirstOrDefaultAsync(l => l.Code == "B-01");

        if (prd1 == null || prd2 == null || locA01 == null || locB01 == null) return;

        var now = DateTime.UtcNow;
        var stocks = new[]
        {
            new StockLevel { ProductId = prd1.Id, WarehouseLocationId = locA01.Id, Quantity = 150, CreatedAt = now },
            new StockLevel { ProductId = prd2.Id, WarehouseLocationId = locB01.Id, Quantity = 50, CreatedAt = now }
        };
        dbContext.StockLevels.AddRange(stocks);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedStockMovementsAsync(AppDbContext dbContext)
    {
        if (await dbContext.StockMovements.AnyAsync()) return;

        var prd1 = await dbContext.Products.FirstOrDefaultAsync(p => p.Sku == "PRD-001");
        var locA01 = await dbContext.WarehouseLocations.FirstOrDefaultAsync(l => l.Code == "A-01");
        var spl1 = await dbContext.Suppliers.FirstOrDefaultAsync(s => s.Code == "SPL-001");
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "operator@example.com");

        if (prd1 == null || locA01 == null || spl1 == null || user == null) return;

        var now = DateTime.UtcNow;

        var movement = new StockMovement
        {
            MovementNumber = $"IN-{now:yyyyMMddHHmmss}-SEED",
            Type = WarehouseWeb.Api.Models.Enums.StockMovementType.Inbound,
            Status = WarehouseWeb.Api.Models.Enums.StockMovementStatus.Completed,
            SupplierId = spl1.Id,
            Notes = "Seeded Initial Stock",
            CreatedByUserId = user.Id,
            CreatedAt = now,
            CompletedAt = now,
            Items = new List<StockMovementItem>
            {
                new StockMovementItem
                {
                    ProductId = prd1.Id,
                    DestinationLocationId = locA01.Id,
                    Quantity = 150
                }
            }
        };

        dbContext.StockMovements.Add(movement);
        await dbContext.SaveChangesAsync();
    }
}
