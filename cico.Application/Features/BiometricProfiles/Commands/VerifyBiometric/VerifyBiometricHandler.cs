using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Commands.VerifyBiometric;

public class VerifyBiometricHandler
    : IRequestHandler<
        VerifyBiometricCommand,
        VerifyBiometricResult>
{
    private readonly IBiometricProfileRepository _repository;

    public VerifyBiometricHandler(
        IBiometricProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<VerifyBiometricResult> Handle(
        VerifyBiometricCommand request,
        CancellationToken cancellationToken)
    {
        var profile =
            await _repository
                .GetByEmployeeAndTypeAsync(
                    request.EmployeeId,
                    request.Type);

        if (profile == null)
            throw new DomainException(
                "No biometric profile registered");

        if (!profile.IsActive)
            throw new DomainException(
                "Biometric profile is disabled");

        var isMatch = profile.Template ==
            request.ScannedTemplate;

        if (!isMatch)
        {
            return new VerifyBiometricResult
            {
                IsMatch = false,
                Confidence = 0,
                Message = "Biometric does not match"
            };
        }

        profile.LastVerifiedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(profile);

        return new VerifyBiometricResult
        {
            IsMatch = true,
            Confidence = 100,
            Message = "Verification successful"
        };
    }
}
