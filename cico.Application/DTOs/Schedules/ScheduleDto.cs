namespace cico.Application.DTOs.Schedules;

public class ScheduleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int LateThresholdMinutes { get; set; }

    public bool IsActive { get; set; }
}
