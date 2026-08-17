
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockLevels;

namespace WarehouseWeb.Api.Services
{
    public interface IStockLevelService
    {
        Task<PaginatedResponse<StockLevelDto>> ListAsync(StockLevelQueryRequest request);
        Task<IReadOnlyList<StockLevelDto>> ListByProductAsync(Guid productId);
        Task<IReadOnlyList<StockLevelDto>> ListByLocationAsync(Guid locationId);
    }
}
