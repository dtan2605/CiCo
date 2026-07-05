using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class ScheduleRequest : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public DateOnly RequestDate { get; set; }
    public Guid? CurrentScheduleId { get; set; }
    public Schedule? CurrentSchedule { get; set; }
    public Guid? RequestedScheduleId { get; set; }
    public Schedule? RequestedSchedule { get; set; }

    public string Reason { get; set; } = string.Empty;

    public ScheduleRequestStatus Status { get; set; } = ScheduleRequestStatus.Pending;

    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
