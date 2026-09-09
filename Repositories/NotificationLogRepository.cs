using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> ExistsByMovementIdAsync(Guid movementId)
        {
            return await _dbContext.NotificationLogs
                .AnyAsync(n => n.RelatedMovementId == movementId);
        }

        public async Task<List<NotificationLog>> ListByRecipientAsync(Guid userId, string role, int offset, int limit)
        {
            var roleName = role.ToLowerInvariant();
            return await _dbContext.NotificationLogs
                .AsNoTracking()
                .Where(n => n.RecipientUserId == userId || n.TargetRole.ToLower() == roleName)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> CountByRecipientAsync(Guid userId, string role)
        {
            var roleName = role.ToLowerInvariant();
            return await _dbContext.NotificationLogs
                .Where(n => n.RecipientUserId == userId || n.TargetRole.ToLower() == roleName)
                .CountAsync();
        }

        public async Task<NotificationLog?> FindByIdAsync(Guid id)
        {
            return await _dbContext.NotificationLogs.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task UpdateAsync(NotificationLog log)
        {
            _dbContext.NotificationLogs.Update(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(NotificationLog log)
        {
            _dbContext.NotificationLogs.Remove(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
