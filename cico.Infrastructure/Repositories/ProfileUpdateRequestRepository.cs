using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class ProfileUpdateRequestRepository
    : IProfileUpdateRequestRepository
{
    private readonly CICODbContext _context;

    public ProfileUpdateRequestRepository(CICODbContext context)
    {
        _context = context;
    }

    public async Task<ProfileUpdateRequest?> GetByIdAsync(Guid id)
    {
        return await _context.ProfileUpdateRequests
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ProfileUpdateRequest>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.ProfileUpdateRequests
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ProfileUpdateRequest>> GetPendingAsync()
    {
        return await _context.ProfileUpdateRequests
            .Include(x => x.Employee)
            .Where(x => x.Status == ProfileUpdateStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ProfileUpdateRequest entity)
    {
        await _context.ProfileUpdateRequests.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProfileUpdateRequest entity)
    {
        _context.ProfileUpdateRequests.Update(entity);
        await _context.SaveChangesAsync();
    }

    public Task<List<ProfileUpdateRequest>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(ProfileUpdateRequest entity)
    {
        throw new NotImplementedException();
    }

    public void Update(ProfileUpdateRequest entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(ProfileUpdateRequest entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
