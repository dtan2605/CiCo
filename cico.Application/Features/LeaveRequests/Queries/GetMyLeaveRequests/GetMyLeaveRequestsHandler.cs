using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.LeaveRequests;

namespace cico.Application.Features.LeaveRequests.Queries.GetMyLeaveRequests;

public class GetMyLeaveRequestsHandler
    : IRequestHandler<GetMyLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly ILeaveRequestRepository _repo;

    public GetMyLeaveRequestsHandler(
        ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<LeaveRequestDto>> Handle(
        GetMyLeaveRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _repo.GetByEmployeeIdAsync(request.EmployeeId);

        return requests.Select(r => new LeaveRequestDto
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
