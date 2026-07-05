using MediatR;

namespace cico.Application.Features.LeaveRequests.Commands.CreateLeaveRequest;

public record CreateLeaveRequestCommand(
    Guid EmployeeId,
    DateOnly FromDate,
    DateOnly ToDate,
    string Reason
) : IRequest<Guid>;
