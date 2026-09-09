namespace WarehouseWeb.Api.DTOs.Notifications;

public class CreateNotificationRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TargetRole { get; set; }
    public Guid? RecipientUserId { get; set; }
}
