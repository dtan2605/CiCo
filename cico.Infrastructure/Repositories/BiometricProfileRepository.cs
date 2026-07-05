using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class BiometricProfileRepository
    : BaseRepository<BiometricProfile>,
      IBiometricProfileRepository
{
    public BiometricProfileRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public override async Task<List<BiometricProfile>>
        GetAllAsync()
    {
        return await _context.BiometricProfiles
            .Include(x => x.Employee)
            .OrderBy(x => x.Employee.FullName)
            .ToListAsync();
    }

    public override async Task<BiometricProfile?>
        GetByIdAsync(Guid id)
    {
        return await _context.BiometricProfiles
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<BiometricProfile>>
        GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.BiometricProfiles
            .Include(x => x.Employee)
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<BiometricProfile?>
        GetByEmployeeAndTypeAsync(
            Guid employeeId,
            BiometricType type)
    {
        return await _context.BiometricProfiles
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeId &&
                x.Type == type);
    }

    public async Task<IReadOnlyList<BiometricProfile>>
        GetActiveProfilesAsync()
    {
        return await _context.BiometricProfiles
            .Include(x => x.Employee)
            .Where(x => x.IsActive)
            .ToListAsync();
    }
}
