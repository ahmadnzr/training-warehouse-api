using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class JobExecutionLogRepository : IJobExecutionLogRepository
    {
        private readonly AppDbContext _dbContext;

        public JobExecutionLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(JobExecutionLog log)
        {
            await _dbContext.JobExecutionLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobExecutionLog log)
        {
            _dbContext.JobExecutionLogs.Update(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<JobExecutionLog?> FindByIdAsync(Guid id)
        {
            return await _dbContext.JobExecutionLogs
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<JobExecutionLog>> ListAsync(int offset, int limit, string sort, string order)
        {
            var query = _dbContext.JobExecutionLogs.AsQueryable();

            query = sort.ToLowerInvariant() switch
            {
                "job_name" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(l => l.JobName)
                    : query.OrderBy(l => l.JobName),
                "status" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(l => l.Status)
                    : query.OrderBy(l => l.Status),
                "finished_at" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(l => l.FinishedAt)
                    : query.OrderBy(l => l.FinishedAt),
                _ => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(l => l.StartedAt)
                    : query.OrderBy(l => l.StartedAt)
            };

            return await query.Skip(offset).Take(limit).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.JobExecutionLogs.CountAsync();
        }
    }
}
