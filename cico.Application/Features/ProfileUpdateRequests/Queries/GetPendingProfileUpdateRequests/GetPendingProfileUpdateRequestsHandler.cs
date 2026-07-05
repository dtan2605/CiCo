using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.ProfileUpdateRequests;

namespace cico.Application.Features.ProfileUpdateRequests.Queries.GetPendingProfileUpdateRequests;

public class GetPendingProfileUpdateRequestsHandler
    : IRequestHandler<GetPendingProfileUpdateRequestsQuery, List<ProfileUpdateRequestDto>>
{
    private readonly IProfileUpdateRequestRepository _repo;

    public GetPendingProfileUpdateRequestsHandler(
        IProfileUpdateRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProfileUpdateRequestDto>> Handle(
        GetPendingProfileUpdateRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var pending = await _repo.GetPendingAsync();

        return pending.Select(r => new ProfileUpdateRequestDto
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
