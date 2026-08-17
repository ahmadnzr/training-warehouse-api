using WarehouseWeb.Api.Common;

namespace WarehouseWeb.Api.DTOs.StockLevels
{
    public class StockLevelQueryRequest : PaginationRequest
    {
        public Guid? ProductId { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
