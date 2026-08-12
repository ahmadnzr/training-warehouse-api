using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IWarehouseLocationRepository
    {
        Task<IEnumerable<WarehouseLocation>> ListAsync(Guid warehouseId, string? search, int offset, int limit, string sort, string order);
        Task<int> CountAsync(Guid warehouseId, string? search);
        Task<WarehouseLocation?> FindByIdAsync(Guid id);
        Task<bool> ExistsByCodeAsync(Guid warehouseId, string code);
        Task AddAsync(WarehouseLocation location);
        Task UpdateAsync(WarehouseLocation location);
    }
}
