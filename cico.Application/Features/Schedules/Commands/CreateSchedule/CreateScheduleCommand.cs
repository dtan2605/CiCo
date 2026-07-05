using MediatR;
using cico.Application.DTOs.Schedules;

namespace cico.Application.Features.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int LateThresholdMinutes
) : IRequest<ScheduleDto>;
