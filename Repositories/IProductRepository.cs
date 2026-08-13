using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> ListAsync(string? search, int offset, int limit, string sort, string order);
        Task<int> CountAsync(string? search);
        Task<Product?> FindByIdAsync(Guid id);
        Task<bool> ExistsBySkuAsync(string sku);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
    }
}
