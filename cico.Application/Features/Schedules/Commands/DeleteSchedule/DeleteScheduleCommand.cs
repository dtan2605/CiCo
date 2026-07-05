using MediatR;

namespace cico.Application.Features.Schedules.Commands.DeleteSchedule;

public record DeleteScheduleCommand(
    Guid Id
) : IRequest;
