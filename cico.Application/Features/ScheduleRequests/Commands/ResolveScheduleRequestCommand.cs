using MediatR;
using cico.Domain.Enums;

namespace cico.Application.Features.ScheduleRequests.Commands;

public record ResolveScheduleRequestCommand(
    Guid Id,
    ScheduleRequestStatus Status,
    string? AdminNote
) : IRequest;
