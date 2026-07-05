using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class ScheduleRequestRepository : IScheduleRequestRepository
{
    private readonly CICODbContext _context;

    public ScheduleRequestRepository(CICODbContext context)
    {
        _context = context;
    }

    public async Task<List<ScheduleRequest>> GetPagingAsync(int pageNumber, int pageSize, Guid? employeeId, ScheduleRequestStatus? status)
    {
        var query = _context.ScheduleRequests
            .Include(x => x.Employee)
            .Include(x => x.CurrentSchedule)
            .Include(x => x.RequestedSchedule)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(x => x.EmployeeId == employeeId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ScheduleRequest?> GetByIdAsync(Guid id)
    {
        return await _context.ScheduleRequests
            .Include(x => x.Employee)
            .Include(x => x.CurrentSchedule)
            .Include(x => x.RequestedSchedule)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ScheduleRequest request)
    {
        await _context.ScheduleRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ScheduleRequest request)
    {
        _context.ScheduleRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}
