using MediatR;
using cico.Application.DTOs.BiometricProfiles;
using cico.Domain.Enums;

namespace cico.Application.Features.BiometricProfiles
    .Commands.RegisterBiometric;

public record RegisterBiometricCommand(
    Guid EmployeeId,
    BiometricType Type,
    string Template
) : IRequest<BiometricProfileDto>;
