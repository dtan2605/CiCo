using MediatR;

namespace cico.Application.Features.BiometricProfiles
    .Commands.DeleteBiometricProfile;

public record DeleteBiometricProfileCommand(
    Guid Id
) : IRequest;
