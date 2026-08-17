using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Models
{
    public class StockMovement
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string MovementNumber { get; set; } = string.Empty;
        public StockMovementType Type { get; set; }
        public StockMovementStatus Status { get; set; } = StockMovementStatus.Draft;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public string? Notes { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<StockMovementItem> Items { get; set; } = new List<StockMovementItem>();
    }
}
