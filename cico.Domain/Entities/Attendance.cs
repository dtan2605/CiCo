using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class Attendance : AuditableEntity
{
    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int LateMinutes { get; set; }

    public AttendanceStatus Status { get; set; }

    public AttendanceMethod Method { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; }
        = null!;

    public Guid? EmployeeScheduleId { get; set; }

    public EmployeeSchedule? EmployeeSchedule { get; set; }

    public ICollection<AttendanceLog> Logs
        { get; set; }
        = new List<AttendanceLog>();
}