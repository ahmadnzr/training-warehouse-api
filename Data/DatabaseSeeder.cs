using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Helpers;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await SeedUsersAsync(dbContext);
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
