using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly AppDbContext _dbContext;


        public StockMovementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<StockMovement> BuildQuery(
            StockMovementType? type,
            StockMovementStatus? status,
            Guid? productId,
            DateTime? dateFrom,
            DateTime? dateTo,
            Guid? createdByUserId)
        {
            var query = _dbContext.StockMovements
                .Where(m => m.DeletedAt == null)
                .AsQueryable();

            if (type.HasValue)
                query = query.Where(m => m.Type == type.Value);

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            if (productId.HasValue)
                query = query.Where(m => m.Items.Any(i => i.ProductId == productId.Value));

            if (dateFrom.HasValue)
                query = query.Where(m => m.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(m => m.CreatedAt <= dateTo.Value);

            // warehouse_operator: hanya movement miliknya
            if (createdByUserId.HasValue)
                query = query.Where(m => m.CreatedByUserId == createdByUserId.Value);

            return query;
        }

        public async Task<StockMovement?> FindByIdAsync(Guid id)
        {
            return await _dbContext.StockMovements
                .Include(m => m.Items)
                .FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);
        }

        public async Task AddAsync(StockMovement movement)
        {
            await _dbContext.StockMovements.AddAsync(movement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<IEnumerable<StockMovement>> ListAsync(StockMovementType? type, StockMovementStatus? status, Guid? productId, DateTime? dateFrom, DateTime? dateTo, Guid? createdByUserId, int offset, int limit, string sort, string order)
        {
            var query = BuildQuery(type, status, productId, dateFrom, dateTo, createdByUserId);

            query = sort.ToLowerInvariant() switch
            {
                "movement_number" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(m => m.MovementNumber)
                    : query.OrderBy(m => m.MovementNumber),
                "status" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(m => m.Status)
                    : query.OrderBy(m => m.Status),
                _ => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(m => m.CreatedAt)
                    : query.OrderBy(m => m.CreatedAt)
            };

            return await query
                .Include(m => m.Items)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> CountAsync(StockMovementType? type, StockMovementStatus? status, Guid? productId, DateTime? dateFrom, DateTime? dateTo, Guid? createdByUserId)
        {

            return await BuildQuery(type, status, productId, dateFrom, dateTo, createdByUserId).CountAsync();
        }

        public async Task UpdateAsync(StockMovement movement)
        {
            _dbContext.StockMovements.Update(movement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<StockLevel?> GetStockLevelAsync(Guid productId, Guid locationId)
        {
            return await _dbContext.StockLevels
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseLocationId == locationId);
        }

        public async Task AddStockLevelAsync(StockLevel stockLevel)
        {
            await _dbContext.StockLevels.AddAsync(stockLevel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStockLevelAsync(StockLevel stockLevel)
        {
            _dbContext.StockLevels.Update(stockLevel);
            await _dbContext.SaveChangesAsync();
        }
    }

}
