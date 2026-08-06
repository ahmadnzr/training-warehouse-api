using Microsoft.EntityFrameworkCore;
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

        var seedUsers = new[]
        {
            new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                PasswordHash = "TEMP_PASSWORD_HASH_CHANGE_WHEN_AUTH_IS_IMPLEMENTED",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Name = "Supervisor",
                Email = "supervisor@example.com",
                PasswordHash = "TEMP_PASSWORD_HASH_CHANGE_WHEN_AUTH_IS_IMPLEMENTED",
                Role = UserRole.Supervisor,
                IsActive = true,
                CreatedAt = now
            },
            new User
            {
                Name = "Warehouse Operator",
                Email = "operator@example.com",
                PasswordHash = "TEMP_PASSWORD_HASH_CHANGE_WHEN_AUTH_IS_IMPLEMENTED",
                Role = UserRole.WarehouseOperator,
                IsActive = true,
                CreatedAt = now
            }
        };

        foreach (var seedUser in seedUsers)
        {
            var exists = await dbContext.Users.AnyAsync(user => user.Email == seedUser.Email);

            if (!exists)
            {
                dbContext.Users.Add(seedUser);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
