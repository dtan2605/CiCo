using cico.Domain.Common;

namespace cico.Domain.Entities;

public class Schedule : AuditableEntity
{
    public string Name { get; set; }
        = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int LateThresholdMinutes { get; set; }

    public bool IsActive { get; set; }
        = true;

    public ICollection<EmployeeSchedule> EmployeeSchedules
        { get; set; }
        = new List<EmployeeSchedule>();
        
}