using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _dbContext;

        public SupplierRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Supplier>> ListAsync(string? search, int offset, int limit, string sort, string order)
        {
            var query = _dbContext.Suppliers.Where(s => s.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Code.Contains(search) || s.Name.Contains(search) || s.Email.Contains(search));
            }

            query = sort.ToLower() switch
            {
                "code" => order.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Code) : query.OrderBy(s => s.Code),
                "name" => order.ToUpper() == "DESC" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                _ => query.OrderByDescending(s => s.CreatedAt)
            };

            return await query.Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<int> CountAsync(string? search)
        {
            var query = _dbContext.Suppliers.Where(s => s.DeletedAt == null).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Code.Contains(search) || s.Name.Contains(search) || s.Email.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<Supplier?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbContext.Suppliers.AnyAsync(s => s.Code == code && s.DeletedAt == null);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _dbContext.Suppliers.AddAsync(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }
    }
}
