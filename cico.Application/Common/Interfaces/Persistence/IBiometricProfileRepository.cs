using cico.Domain.Entities;
using cico.Domain.Enums;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IBiometricProfileRepository
    : IBaseRepository<BiometricProfile>
{
    Task<IReadOnlyList<BiometricProfile>>
        GetByEmployeeIdAsync(Guid employeeId);

    Task<BiometricProfile?> GetByEmployeeAndTypeAsync(
        Guid employeeId,
        BiometricType type);

    Task<IReadOnlyList<BiometricProfile>>
        GetActiveProfilesAsync();
}
