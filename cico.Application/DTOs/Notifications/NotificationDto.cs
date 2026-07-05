using cico.Domain.Enums;

namespace cico.Application.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }
        = string.Empty;

    public string Content { get; set; }
        = string.Empty;

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public Guid EmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }
}
