using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IJobExecutionLogRepository
    {
        Task AddAsync(JobExecutionLog log);
        Task UpdateAsync(JobExecutionLog log);
        Task<JobExecutionLog?> FindByIdAsync(Guid id);

        Task<IEnumerable<JobExecutionLog>> ListAsync(int offset, int limit, string sort, string order);
        Task<int> CountAsync();
    }
}
