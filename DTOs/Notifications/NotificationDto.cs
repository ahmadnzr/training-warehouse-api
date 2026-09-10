namespace WarehouseWeb.Api.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public Guid? RelatedMovementId { get; set; }
    public Guid? RecipientUserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}
