using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> ListAsync(string? search, int offset, int limit, string sort, string order);
        Task<int> CountAsync(string? search);
        Task<Supplier?> FindByIdAsync(Guid id);
        Task<bool> ExistsByCodeAsync(string code);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
    }
}
