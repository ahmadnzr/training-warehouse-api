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
        await SeedCategoriesAsync(dbContext);
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
}
