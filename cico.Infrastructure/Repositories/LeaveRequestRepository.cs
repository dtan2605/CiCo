using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class LeaveRequestRepository
    : ILeaveRequestRepository
{
    private readonly CICODbContext _context;

    public LeaveRequestRepository(CICODbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
    {
        return await _context.LeaveRequests
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.LeaveRequests
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<LeaveRequest>> GetPendingAsync()
    {
        return await _context.LeaveRequests
            .Include(x => x.Employee)
            .Where(x => x.Status == LeaveStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(LeaveRequest entity)
    {
        await _context.LeaveRequests.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LeaveRequest entity)
    {
        _context.LeaveRequests.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LeaveRequest>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .Include(x => x.Employee)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAsync(LeaveRequest entity)
    {
        _context.LeaveRequests.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(LeaveRequest entity)
    {
        _context.LeaveRequests.Update(entity);
    }

    public void Delete(LeaveRequest entity)
    {
        _context.LeaveRequests.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.LeaveRequests.AnyAsync(x => x.Id == id);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
