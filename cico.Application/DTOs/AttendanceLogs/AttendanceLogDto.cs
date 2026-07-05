using cico.Domain.Enums;

namespace cico.Application.DTOs.AttendanceLogs;

public class AttendanceLogDto
{
    public Guid Id { get; set; }

    public DateTime ScanTime { get; set; }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }
        = string.Empty;

    public Guid AttendanceId { get; set; }

    public string AttendanceDate { get; set; }
        = string.Empty;

    public string EmployeeName { get; set; }
        = string.Empty;

    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; }
        = string.Empty;

    public string DeviceCode { get; set; }
        = string.Empty;

    public AttendanceMethod Method { get; set; }
}
