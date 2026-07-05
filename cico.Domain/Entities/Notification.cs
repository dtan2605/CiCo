using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class Notification : AuditableEntity
{
    public string Title { get; set; }
        = string.Empty;

    public string Content { get; set; }
        = string.Empty;

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; }
        = null!;
}