using MediatR;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.UpdateEmployeeSchedule;

public record UpdateEmployeeScheduleCommand(
    Guid Id,
    Guid EmployeeId,
    Guid ScheduleId,
    DateOnly WorkDate,
    bool IsOvertime,
    string? Note
) : IRequest;
