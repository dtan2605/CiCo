using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IPermissionRepository
{
    Task<List<Permission>>
        GetByRoleIdAsync(Guid roleId);
}