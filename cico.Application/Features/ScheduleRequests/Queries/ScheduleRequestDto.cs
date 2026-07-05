namespace cico.Application.Features.ScheduleRequests.Queries;

public class ScheduleRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
    public string? CurrentScheduleName { get; set; }
    public string? RequestedScheduleName { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? AdminNote { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string? ResolvedAt { get; set; }
}
