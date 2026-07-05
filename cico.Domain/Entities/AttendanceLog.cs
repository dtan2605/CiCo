using cico.Domain.Common;
using cico.Domain.Enums;

namespace cico.Domain.Entities;

public class AttendanceLog : AuditableEntity
{
    public DateTime ScanTime { get; set; }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }
        = string.Empty;

    public Guid AttendanceId { get; set; }

    public Attendance Attendance { get; set; }
        = null!;

    public Guid DeviceId { get; set; }

    public Device Device { get; set; }
        = null!;

    public AttendanceMethod Method { get; set; }
}