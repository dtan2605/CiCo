using Microsoft.EntityFrameworkCore;
using cico.Domain.Entities;
using cico.Application.Common.Interfaces.Persistence;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class PermissionRepository
    : IPermissionRepository
{
    private readonly CICODbContext
        _context;

    public PermissionRepository(
        CICODbContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>>
        GetByRoleIdAsync(Guid roleId)
    {
        return await _context
            .RolePermissions
            .Where(x =>
                x.RoleId == roleId)
            .Select(x =>
                x.Permission)
            .ToListAsync();
    }
}