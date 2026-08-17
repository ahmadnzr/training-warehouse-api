using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly AppDbContext _dbContext;

        public StockMovementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }

}
