using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class WarehouseLocationRepository : IWarehouseLocationRepository
    {
        private readonly AppDbContext _dbContext;

        public WarehouseLocationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WarehouseLocation>> ListAsync(Guid warehouseId, string? search, int offset, int limit, string sort, string order)
        {
            var query = _dbContext.WarehouseLocations
                .Include(w => w.Warehouse)
                .Where(w => w.DeletedAt == null && w.WarehouseId == warehouseId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(w => w.Name.Contains(search) || w.Code.Contains(search));
            }

            query = sort.ToLower() switch
            {
                "name" => order.ToUpper() == "DESC" ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
                "code" => order.ToUpper() == "DESC" ? query.OrderByDescending(w => w.Code) : query.OrderBy(w => w.Code),
                _ => query.OrderByDescending(w => w.CreatedAt)
            };

            return await query.Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<int> CountAsync(Guid warehouseId, string? search)
        {
            var query = _dbContext.WarehouseLocations.Where(w => w.DeletedAt == null && w.WarehouseId == warehouseId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(w => w.Name.Contains(search) || w.Code.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<WarehouseLocation?> FindByIdAsync(Guid id)
        {
            return await _dbContext.WarehouseLocations
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(w => w.Id == id && w.DeletedAt == null);
        }

        public async Task<bool> ExistsByCodeAsync(Guid warehouseId, string code)
        {
            return await _dbContext.WarehouseLocations
                .AnyAsync(w => w.WarehouseId == warehouseId && w.Code == code && w.DeletedAt == null);
        }

        public async Task AddAsync(WarehouseLocation location)
        {
            await _dbContext.WarehouseLocations.AddAsync(location);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(WarehouseLocation location)
        {
            _dbContext.WarehouseLocations.Update(location);
            await _dbContext.SaveChangesAsync();
        }
    }
}
