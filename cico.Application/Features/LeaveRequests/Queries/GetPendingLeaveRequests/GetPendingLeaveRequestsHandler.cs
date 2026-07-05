using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.LeaveRequests;

namespace cico.Application.Features.LeaveRequests.Queries.GetPendingLeaveRequests;

public class GetPendingLeaveRequestsHandler
    : IRequestHandler<GetPendingLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly ILeaveRequestRepository _repo;

    public GetPendingLeaveRequestsHandler(
        ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<LeaveRequestDto>> Handle(
        GetPendingLeaveRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var pending = await _repo.GetPendingAsync();

        return pending.Select(r => new LeaveRequestDto
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            EmployeeName = r.Employee.FullName,
            EmployeeCode = r.Employee.EmployeeCode,
            FromDate = r.FromDate,
            ToDate = r.ToDate,
            Reason = r.Reason,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}
