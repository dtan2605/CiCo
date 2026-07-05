using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IProfileUpdateRequestRepository
    : IBaseRepository<ProfileUpdateRequest>
{
    Task<List<ProfileUpdateRequest>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<ProfileUpdateRequest>> GetPendingAsync();
}
