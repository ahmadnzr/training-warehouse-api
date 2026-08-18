using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IDailyStockReportRepository
    {
        Task<DailyStockReport?> FindByDateAsync(DateOnly reportDate);
        Task<DailyStockReport?> FindByDateWithItemsAsync(DateOnly reportDate);

        Task<IEnumerable<DailyStockReport>> ListAsync(
            DateOnly? dateFrom,
            DateOnly? dateTo,
            int offset,
            int limit,
            string sort,
            string order);

        Task<int> CountAsync(DateOnly? dateFrom, DateOnly? dateTo);

        Task AddAsync(DailyStockReport report);
        Task UpdateAsync(DailyStockReport report);

        /// <summary>
        /// Aggregate live stock: product_id + SUM(quantity) across all locations.
        /// </summary>
        Task<IReadOnlyList<(Guid ProductId, int TotalQuantity)>> AggregateStockByProductAsync();
    }
}
