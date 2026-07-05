using MediatR;
using cico.Domain.Enums;

namespace cico.Application.Features.ProfileUpdateRequests.Commands.ResolveProfileUpdateRequest;

public record ResolveProfileUpdateRequestCommand(
    Guid RequestId,
    ProfileUpdateStatus Status,
    Guid ResolvedByUserId
) : IRequest;
