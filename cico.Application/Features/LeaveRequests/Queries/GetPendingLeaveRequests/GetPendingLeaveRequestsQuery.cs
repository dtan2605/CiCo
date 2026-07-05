using MediatR;
using cico.Application.DTOs.LeaveRequests;

namespace cico.Application.Features.LeaveRequests.Queries.GetPendingLeaveRequests;

public record GetPendingLeaveRequestsQuery()
    : IRequest<List<LeaveRequestDto>>;
