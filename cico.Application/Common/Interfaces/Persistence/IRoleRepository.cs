using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(Guid id);
}
