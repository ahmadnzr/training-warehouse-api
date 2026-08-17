using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IStockLevelRepository
    {
        Task<IEnumerable<StockLevel>> ListAsync(
            Guid? productId,
            Guid? warehouseLocationId,
            Guid? warehouseId,
            int offset,
            int limit,
            string sort,
            string order);

        Task<int> CountAsync(
            Guid? productId,
            Guid? warehouseLocationId,
            Guid? warehouseId);

        Task<IEnumerable<StockLevel>> ListByProductAsync(Guid productId);
        Task<IEnumerable<StockLevel>> ListByLocationAsync(Guid locationId);
    }
}
