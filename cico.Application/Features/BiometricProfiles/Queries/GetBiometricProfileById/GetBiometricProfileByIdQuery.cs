using MediatR;
using cico.Application.DTOs.BiometricProfiles;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfileById;

public record GetBiometricProfileByIdQuery(
    Guid Id
) : IRequest<BiometricProfileDto?>;
