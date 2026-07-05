using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Commands.DeleteBiometricProfile;

public class DeleteBiometricProfileHandler
    : IRequestHandler<DeleteBiometricProfileCommand>
{
    private readonly IBiometricProfileRepository _repository;

    public DeleteBiometricProfileHandler(
        IBiometricProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteBiometricProfileCommand request,
        CancellationToken cancellationToken)
    {
        var profile =
            await _repository.GetByIdAsync(
                request.Id);

        if (profile == null)
            throw new DomainException(
                "Biometric profile not found");

        await _repository.DeleteAsync(profile);
    }
}
