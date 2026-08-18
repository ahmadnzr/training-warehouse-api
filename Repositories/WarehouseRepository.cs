using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _dbContext;

    public WarehouseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Warehouse?> FindByIdAsync(Guid id)
    {
        return await _dbContext.Warehouses
            .Include(w => w.Locations.Where(l => l.DeletedAt == null))
            .FirstOrDefaultAsync(w => w.Id == id && w.DeletedAt == null);
    }

    public async Task<Warehouse?> FindByCodeAsync(string code)
    {
        return await _dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.Code == code && w.DeletedAt == null);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
    {
        var query = _dbContext.Warehouses
            .Where(w => w.Code == code && w.DeletedAt == null);

        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        await _dbContext.Warehouses.AddAsync(warehouse);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        warehouse.UpdatedAt = DateTime.UtcNow;
        _dbContext.Warehouses.Update(warehouse);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Warehouse>> ListAsync(string? search, int skip, int take, string sort, string order)
    {
        var query = _dbContext.Warehouses
            .Where(w => w.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(w =>
                EF.Functions.Like(w.Code, $"%{search}%") ||
                EF.Functions.Like(w.Name, $"%{search}%") ||
                EF.Functions.Like(w.City!, $"%{search}%"));
        }

        var isAsc = order.Equals("ASC", StringComparison.OrdinalIgnoreCase);

        query = sort.ToLowerInvariant() switch
        {
            "code" => isAsc ? query.OrderBy(w => w.Code) : query.OrderByDescending(w => w.Code),
            "name" => isAsc ? query.OrderBy(w => w.Name) : query.OrderByDescending(w => w.Name),
            "city" => isAsc ? query.OrderBy(w => w.City) : query.OrderByDescending(w => w.City),
            "created_at" => isAsc ? query.OrderBy(w => w.CreatedAt) : query.OrderByDescending(w => w.CreatedAt),
            _ => isAsc ? query.OrderBy(w => w.UpdatedAt) : query.OrderByDescending(w => w.UpdatedAt)
        };

        return await query
            .Include(w => w.Locations.Where(l => l.DeletedAt == null))
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? search)
    {
        var query = _dbContext.Warehouses
            .Where(w => w.DeletedAt == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(w =>
                EF.Functions.Like(w.Code, $"%{search}%") ||
                EF.Functions.Like(w.Name, $"%{search}%") ||
                EF.Functions.Like(w.City!, $"%{search}%"));
        }

        return await query.CountAsync();
    }
}
