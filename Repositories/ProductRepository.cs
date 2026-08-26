using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<bool> ExistsBySkuAsync(string sku)
        {
            return await _dbContext.Products
                .AnyAsync(p => p.Sku == sku);
        }


        public async Task<IEnumerable<Product>> ListAsync(string? search, int offset, int limit, string sort, string order)
        {
            var query = _dbContext.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"{search}%") ||
                    EF.Functions.Like(p.Sku, $"{search}%"));
            }

            query = sort.ToLower() switch
            {
                "name" => order.ToUpper() == "DESC" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "sku" => order.ToUpper() == "DESC" ? query.OrderByDescending(p => p.Sku) : query.OrderBy(p => p.Sku),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await query.Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<int> CountAsync(string? search)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"{search}%") ||
                    EF.Functions.Like(p.Sku, $"{search}%"));
            }

            return await query.CountAsync();
        }

        public async Task<Product?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> FindByIdsAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.Distinct().ToList();
            if (!idList.Any()) return new List<Product>();

            return await _dbContext.Products
                .Where(p => idList.Contains(p.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }


    }
}
