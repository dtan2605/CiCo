using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.ProfileUpdateRequests;

namespace cico.Application.Features.ProfileUpdateRequests.Queries.GetMyProfileUpdateRequests;

public class GetMyProfileUpdateRequestsHandler
    : IRequestHandler<GetMyProfileUpdateRequestsQuery, List<ProfileUpdateRequestDto>>
{
    private readonly IProfileUpdateRequestRepository _repo;

    public GetMyProfileUpdateRequestsHandler(
        IProfileUpdateRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProfileUpdateRequestDto>> Handle(
        GetMyProfileUpdateRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _repo.GetByEmployeeIdAsync(request.EmployeeId);

        return requests.Select(r => new ProfileUpdateRequestDto
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            EmployeeName = r.Employee.FullName,
            EmployeeCode = r.Employee.EmployeeCode,
            FullName = r.FullName,
            Email = r.Email,
            PhoneNumber = r.PhoneNumber,
            Address = r.Address,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}
