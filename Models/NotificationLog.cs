using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Models
{
    public class NotificationLog
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        /// <summary>Target role, e.g. supervisor</summary>
        public string TargetRole { get; set; } = "supervisor";

        public Guid? RelatedMovementId { get; set; }
        public Guid? RecipientUserId { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
