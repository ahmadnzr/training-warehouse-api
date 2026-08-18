using WarehouseWeb.Api.Common;

namespace WarehouseWeb.Api.DTOs.Reports
{
    public class DailyStockReportQueryRequest : PaginationRequest
    {
        public DailyStockReportQueryRequest()
        {
            Sort = "report_date";
        }

        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
