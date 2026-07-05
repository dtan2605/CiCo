using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;
public class EmployeeSchedule : AuditableEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; }
        = null!;

    public Guid ScheduleId { get; set; }

    public Schedule Schedule { get; set; }
        = null!;

    public DateOnly WorkDate { get; set; }

    public bool IsOvertime { get; set; }

    public string Note { get; set; }
        = string.Empty;
}