using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class LeaveRequest : AuditableEntity
{
    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public LeaveStatus Status { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; }
        = null!;
}