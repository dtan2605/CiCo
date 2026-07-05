using MediatR;
using cico.Application.DTOs.ProfileUpdateRequests;

namespace cico.Application.Features.ProfileUpdateRequests.Queries.GetPendingProfileUpdateRequests;

public record GetPendingProfileUpdateRequestsQuery()
    : IRequest<List<ProfileUpdateRequestDto>>;
