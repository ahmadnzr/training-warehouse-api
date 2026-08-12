using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _dbContext;

    public ProductCategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category?> FindByIdAsync(Guid id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    public async Task<Category?> FindByNameAsync(string name)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Name == name && c.DeletedAt == null);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        var query = _dbContext.Categories
            .Where(c => c.Name == name && c.DeletedAt == null);

        // Jika excludeId ada (sedang update), jangan cek ID milik sendiri
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Category>> ListAsync(string? search, int skip, int take, string sort, string order)
    {
        var query = _dbContext.Categories
            .Where(c => c.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{search}%"));
        }

        var isAsc = order.Equals("ASC", StringComparison.OrdinalIgnoreCase);

        query = sort.ToLowerInvariant() switch
        {
            "name" => isAsc ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "created_at" => isAsc ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
            _ => isAsc ? query.OrderBy(c => c.UpdatedAt) : query.OrderByDescending(c => c.UpdatedAt)
        };

        return await query.Skip(skip).Take(take).ToListAsync();
    }

    public async Task<int> CountAsync(string? search)
    {
        var query = _dbContext.Categories
            .Where(c => c.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{search}%"));
        }

        return await query.CountAsync();
    }
}
