namespace cico.Application.DTOs.EmployeeSchedules;

public class EmployeeScheduleDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public Guid ScheduleId { get; set; }

    public string ScheduleName { get; set; }
        = string.Empty;

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsOvertime { get; set; }

    public string Note { get; set; }
        = string.Empty;
}
