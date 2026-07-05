using MediatR;

namespace cico.Application.Features.Schedules.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    Guid Id,
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int LateThresholdMinutes,
    bool IsActive
) : IRequest;
