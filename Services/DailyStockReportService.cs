using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Jobs;
using WarehouseWeb.Api.DTOs.Reports;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class DailyStockReportService : IDailyStockReportService
    {
        private readonly IDailyStockReportRepository _reportRepository;
        private readonly IJobExecutionLogRepository _jobLogRepository;

        public DailyStockReportService(
            IDailyStockReportRepository reportRepository,
            IJobExecutionLogRepository jobLogRepository)
        {
            _reportRepository = reportRepository;
            _jobLogRepository = jobLogRepository;
        }

        public async Task<PaginatedResponse<DailyStockReportDto>> ListAsync(DailyStockReportQueryRequest request)
        {
            request.Validate();

            var items = await _reportRepository.ListAsync(
                request.DateFrom,
                request.DateTo,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order);

            var total = await _reportRepository.CountAsync(request.DateFrom, request.DateTo);

            return new PaginatedResponse<DailyStockReportDto>
            {
                Items = items.Select(MapToListDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = request.Page,
                    PerPage = request.PerPage,
                    Total = total,
                    TotalPage = (int)Math.Ceiling(total / (double)request.PerPage)
                }
            };
        }

        public async Task<DailyStockReportDetailDto> GetByDateAsync(DateOnly date)
        {
            var report = await _reportRepository.FindByDateWithItemsAsync(date);
            if (report == null)
                throw new NotFoundException("Daily stock report not found");

            return MapToDetailDto(report);
        }

        public async Task<DailyStockReportDetailDto> GenerateAsync(DateOnly? reportDate, Guid? triggeredByUserId = null)
        {
            var date = reportDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var jobLog = new JobExecutionLog
            {
                JobName = JobNames.DailyStockReport,
                Status = JobExecutionStatus.Running,
                StartedAt = DateTime.UtcNow
            };
            await _jobLogRepository.AddAsync(jobLog);

            try
            {
                var aggregates = await _reportRepository.AggregateStockByProductAsync();

                var report = await _reportRepository.FindByDateWithItemsAsync(date);
                if (report == null)
                {
                    report = new DailyStockReport
                    {
                        ReportDate = date,
                        GeneratedAt = DateTime.UtcNow,
                        JobExecutionLogId = jobLog.Id,
                        Items = aggregates.Select(a => new DailyStockReportItem
                        {
                            ProductId = a.ProductId,
                            TotalQuantity = a.TotalQuantity
                        }).ToList()
                    };
                    await _reportRepository.AddAsync(report);
                }
                else
                {
                    report.Items.Clear();
                    foreach (var a in aggregates)
                    {
                        report.Items.Add(new DailyStockReportItem
                        {
                            ProductId = a.ProductId,
                            TotalQuantity = a.TotalQuantity
                        });
                    }

                    report.GeneratedAt = DateTime.UtcNow;
                    report.JobExecutionLogId = jobLog.Id;
                    await _reportRepository.UpdateAsync(report);
                }

                jobLog.Status = JobExecutionStatus.Succeeded;
                jobLog.FinishedAt = DateTime.UtcNow;
                await _jobLogRepository.UpdateAsync(jobLog);

                var saved = await _reportRepository.FindByDateWithItemsAsync(date)
                    ?? throw new NotFoundException("Report not found after generate");

                return MapToDetailDto(saved);
            }
            catch (Exception ex)
            {
                jobLog.Status = JobExecutionStatus.Failed;
                jobLog.FinishedAt = DateTime.UtcNow;
                jobLog.ErrorMessage = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;
                await _jobLogRepository.UpdateAsync(jobLog);
                throw;
            }
        }

        public async Task<PaginatedResponse<JobExecutionLogDto>> ListJobExecutionsAsync(PaginationRequest request)
        {
            request.Validate();

            if (string.IsNullOrWhiteSpace(request.Sort) || request.Sort == "updated_at")
                request.Sort = "started_at";

            var items = await _jobLogRepository.ListAsync(
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order);

            var total = await _jobLogRepository.CountAsync();

            return new PaginatedResponse<JobExecutionLogDto>
            {
                Items = items.Select(MapToJobDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = request.Page,
                    PerPage = request.PerPage,
                    Total = total,
                    TotalPage = (int)Math.Ceiling(total / (double)request.PerPage)
                }
            };
        }

        private static DailyStockReportDto MapToListDto(DailyStockReport entity)
        {
            return new DailyStockReportDto
            {
                Id = entity.Id,
                ReportDate = entity.ReportDate,
                GeneratedAt = entity.GeneratedAt,
                JobExecutionLogId = entity.JobExecutionLogId,
                ItemCount = entity.Items?.Count ?? 0
            };
        }

        private static DailyStockReportDetailDto MapToDetailDto(DailyStockReport entity)
        {
            return new DailyStockReportDetailDto
            {
                Id = entity.Id,
                ReportDate = entity.ReportDate,
                GeneratedAt = entity.GeneratedAt,
                JobExecutionLogId = entity.JobExecutionLogId,
                Items = entity.Items.Select(i => new DailyStockReportItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductSku = i.Product?.Sku,
                    ProductName = i.Product?.Name,
                    TotalQuantity = i.TotalQuantity
                }).ToList()
            };
        }

        private static JobExecutionLogDto MapToJobDto(JobExecutionLog entity)
        {
            return new JobExecutionLogDto
            {
                Id = entity.Id,
                JobName = entity.JobName,
                Status = entity.Status.ToString().ToLowerInvariant(),
                StartedAt = entity.StartedAt,
                FinishedAt = entity.FinishedAt,
                ErrorMessage = entity.ErrorMessage
            };
        }
    }
}
