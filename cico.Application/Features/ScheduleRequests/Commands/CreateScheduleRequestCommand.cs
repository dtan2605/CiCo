using MediatR;

namespace cico.Application.Features.ScheduleRequests.Commands;

public record CreateScheduleRequestCommand(
    Guid EmployeeId,
    DateOnly RequestDate,
    Guid? CurrentScheduleId,
    Guid? RequestedScheduleId,
    string Reason
) : IRequest<Guid>;
