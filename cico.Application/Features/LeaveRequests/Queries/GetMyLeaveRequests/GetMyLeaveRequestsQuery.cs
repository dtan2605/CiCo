using MediatR;
using cico.Application.DTOs.LeaveRequests;

namespace cico.Application.Features.LeaveRequests.Queries.GetMyLeaveRequests;

public record GetMyLeaveRequestsQuery(
    Guid EmployeeId
) : IRequest<List<LeaveRequestDto>>;
