using MediatR;
using cico.Application.DTOs.BiometricProfiles;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesByEmployee;

public record GetBiometricProfilesByEmployeeQuery(
    Guid EmployeeId
) : IRequest<List<BiometricProfileDto>>;
