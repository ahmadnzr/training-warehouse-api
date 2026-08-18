using Coravel.Invocable;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Jobs;

public class DailyStockReportJob : IInvocable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyStockReportJob> _logger;

    public DailyStockReportJob(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyStockReportJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Invoke()
    {
        // Job dijalankan di luar HTTP request → buat scope sendiri
        // supaya scoped DbContext/Service aman dipakai.
        using var scope = _scopeFactory.CreateScope();
        var reportService = scope.ServiceProvider.GetRequiredService<IDailyStockReportService>();

        _logger.LogInformation("DailyStockReportJob started at {Time}", DateTime.UtcNow);

        try
        {
            // null date = hari UTC ini (atau DateOnly.FromDateTime(DateTime.UtcNow.Date))
            var result = await reportService.GenerateAsync(reportDate: null);
            _logger.LogInformation(
                "DailyStockReportJob succeeded. ReportDate={ReportDate}, Items={Count}",
                result.ReportDate,
                result.Items.Count);
        }
        catch (Exception ex)
        {
            // GenerateAsync sudah menulis job log failed; di sini cukup log aplikasi
            _logger.LogError(ex, "DailyStockReportJob failed");
            throw;
        }
    }
}
