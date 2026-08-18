using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly AppDbContext _dbContext;

        public NotificationLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(NotificationLog log)
        {
            await _dbContext.NotificationLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
