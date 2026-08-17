using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IStockMovementRepository
    {
        Task<StockMovement?> FindByIdAsync(Guid id);
        Task AddAsync(StockMovement movement);
    }
}
