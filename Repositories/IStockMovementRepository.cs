using Microsoft.EntityFrameworkCore.Storage;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Repositories
{
    public interface IStockMovementRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<IEnumerable<StockMovement>> ListAsync(
            StockMovementType? type,
            StockMovementStatus? status,
            Guid? productId,
            DateTime? dateFrom,
            DateTime? dateTo,
            Guid? createdByUserId,
            int offset,
            int limit,
            string sort,
            string order);

        Task<int> CountAsync(
            StockMovementType? type,
            StockMovementStatus? status,
            Guid? productId,
            DateTime? dateFrom,
            DateTime? dateTo,
            Guid? createdByUserId);

        Task<StockMovement?> FindByIdAsync(Guid id);
        Task AddAsync(StockMovement movement);

        Task UpdateAsync(StockMovement movement);

        Task<StockLevel?> GetStockLevelAsync(Guid productId, Guid locationId);
        Task AddStockLevelAsync(StockLevel stockLevel);
        Task UpdateStockLevelAsync(StockLevel stockLevel);
    }
}
