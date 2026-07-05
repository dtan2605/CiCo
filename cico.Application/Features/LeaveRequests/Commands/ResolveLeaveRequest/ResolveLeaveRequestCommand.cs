using MediatR;
using cico.Domain.Enums;

namespace cico.Application.Features.LeaveRequests.Commands.ResolveLeaveRequest;

public record ResolveLeaveRequestCommand(
    Guid RequestId,
    LeaveStatus Status
) : IRequest;
