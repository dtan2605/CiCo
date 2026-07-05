using MediatR;
using cico.Application.DTOs.ProfileUpdateRequests;

namespace cico.Application.Features.ProfileUpdateRequests.Queries.GetMyProfileUpdateRequests;

public record GetMyProfileUpdateRequestsQuery(
    Guid EmployeeId
) : IRequest<List<ProfileUpdateRequestDto>>;
