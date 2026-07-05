using cico.Domain.Enums;

namespace cico.Application.DTOs.Attendances;

public class AttendanceDto
{
    public Guid Id { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int LateMinutes { get; set; }

    public AttendanceStatus Status { get; set; }

    public AttendanceMethod Method { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid? EmployeeScheduleId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;
}
