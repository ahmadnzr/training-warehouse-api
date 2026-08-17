using WarehouseWeb.Api.Common;

namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class StockMovementQueryRequest : PaginationRequest
    {
        public StockMovementQueryRequest()
        {
            Sort = "created_at";
        }

        public string? Type { get; set; }
        public string? Status { get; set; }
        public Guid? ProductId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
