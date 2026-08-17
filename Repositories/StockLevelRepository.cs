using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class StockLevelRepository : IStockLevelRepository
    {
        private readonly AppDbContext _dbContext;

        public StockLevelRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StockLevel>> ListAsync(
            Guid? productId,
            Guid? warehouseLocationId,
            Guid? warehouseId,
            int offset,
            int limit,
            string sort,
            string order)
        {
            var query = BuildQuery(productId, warehouseLocationId, warehouseId);

            query = sort.ToLowerInvariant() switch
            {
                "quantity" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(s => s.Quantity)
                    : query.OrderBy(s => s.Quantity),
                "created_at" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(s => s.CreatedAt)
                    : query.OrderBy(s => s.CreatedAt),
                _ => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                    : query.OrderBy(s => s.UpdatedAt ?? s.CreatedAt)
            };

            return await query.Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<int> CountAsync(
            Guid? productId,
            Guid? warehouseLocationId,
            Guid? warehouseId)
        {
            return await BuildQuery(productId, warehouseLocationId, warehouseId).CountAsync();
        }

        public async Task<IEnumerable<StockLevel>> ListByProductAsync(Guid productId)
        {
            return await _dbContext.StockLevels
                .Include(s => s.Product)
                .Include(s => s.WarehouseLocation)
                .Where(s => s.ProductId == productId)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockLevel>> ListByLocationAsync(Guid locationId)
        {
            return await _dbContext.StockLevels
                .Include(s => s.Product)
                .Include(s => s.WarehouseLocation)
                .Where(s => s.WarehouseLocationId == locationId)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();
        }

        private IQueryable<StockLevel> BuildQuery(
            Guid? productId,
            Guid? warehouseLocationId,
            Guid? warehouseId)
        {
            var query = _dbContext.StockLevels
                .Include(s => s.Product)
                .Include(s => s.WarehouseLocation)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(s => s.ProductId == productId.Value);

            if (warehouseLocationId.HasValue)
                query = query.Where(s => s.WarehouseLocationId == warehouseLocationId.Value);

            if (warehouseId.HasValue)
                query = query.Where(s => s.WarehouseLocation != null && s.WarehouseLocation.WarehouseId == warehouseId.Value);

            return query;
        }
    }
}
