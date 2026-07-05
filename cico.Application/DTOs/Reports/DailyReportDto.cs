namespace cico.Application.DTOs.Reports;

public class DailyReportDto
{
    public DateOnly Date { get; set; }
    public int TotalEmployees { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public List<AttendanceBriefDto> Attendances { get; set; } = new();
}

public class AttendanceBriefDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public int LateMinutes { get; set; }
}
