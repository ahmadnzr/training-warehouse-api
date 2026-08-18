using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Data;
using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public class DailyStockReportRepository : IDailyStockReportRepository
    {
        private readonly AppDbContext _dbContext;

        public DailyStockReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DailyStockReport?> FindByDateAsync(DateOnly reportDate)
        {
            return await _dbContext.DailyStockReports
                .FirstOrDefaultAsync(r => r.ReportDate == reportDate);
        }

        public async Task<DailyStockReport?> FindByDateWithItemsAsync(DateOnly reportDate)
        {
            return await _dbContext.DailyStockReports
                .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.ReportDate == reportDate);
        }

        public async Task<IEnumerable<DailyStockReport>> ListAsync(
            DateOnly? dateFrom,
            DateOnly? dateTo,
            int offset,
            int limit,
            string sort,
            string order)
        {
            var query = BuildQuery(dateFrom, dateTo);

            query = sort.ToLowerInvariant() switch
            {
                "generated_at" => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(r => r.GeneratedAt)
                    : query.OrderBy(r => r.GeneratedAt),
                _ => order.ToUpperInvariant() == "DESC"
                    ? query.OrderByDescending(r => r.ReportDate)
                    : query.OrderBy(r => r.ReportDate)
            };

            return await query
                .Include(r => r.Items)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> CountAsync(DateOnly? dateFrom, DateOnly? dateTo)
        {
            return await BuildQuery(dateFrom, dateTo).CountAsync();
        }

        public async Task AddAsync(DailyStockReport report)
        {
            await _dbContext.DailyStockReports.AddAsync(report);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(DailyStockReport report)
        {
            // Do not call .Update() because the entity is already tracked,
            // and .Update() forces all child entities to Modified (causing concurrency issues on new items).
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<(Guid ProductId, int TotalQuantity)>> AggregateStockByProductAsync()
        {
            var rows = await _dbContext.StockLevels
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.ProductId)
                .ToListAsync();

            return rows.Select(x => (x.ProductId, x.TotalQuantity)).ToList();
        }

        private IQueryable<DailyStockReport> BuildQuery(DateOnly? dateFrom, DateOnly? dateTo)
        {
            var query = _dbContext.DailyStockReports.AsQueryable();

            if (dateFrom.HasValue)
                query = query.Where(r => r.ReportDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(r => r.ReportDate <= dateTo.Value);

            return query;
        }
    }
}
