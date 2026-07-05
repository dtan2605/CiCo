using MediatR;
using cico.Domain.Enums;

namespace cico.Application.Features.BiometricProfiles
    .Commands.VerifyBiometric;

public class VerifyBiometricResult
{
    public bool IsMatch { get; set; }
    public float Confidence { get; set; }
    public string Message { get; set; } = string.Empty;
}

public record VerifyBiometricCommand(
    Guid EmployeeId,
    BiometricType Type,
    string ScannedTemplate
) : IRequest<VerifyBiometricResult>;
