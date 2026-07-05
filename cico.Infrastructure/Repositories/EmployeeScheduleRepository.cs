using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class EmployeeScheduleRepository
    : BaseRepository<EmployeeSchedule>,
      IEmployeeScheduleRepository
{
    public EmployeeScheduleRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public override async Task<List<EmployeeSchedule>> GetAllAsync()
    {
        return await _context.EmployeeSchedules
            .Include(x => x.Employee)
            .Include(x => x.Schedule)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync();
    }

    public override async Task<EmployeeSchedule?> GetByIdAsync(
        Guid id)
    {
        return await _context.EmployeeSchedules
            .Include(x => x.Employee)
            .Include(x => x.Schedule)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<EmployeeSchedule>>
        GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.EmployeeSchedules
            .Include(x => x.Employee)
            .Include(x => x.Schedule)
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.WorkDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<EmployeeSchedule>>
        GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.EmployeeSchedules
            .Where(x => x.ScheduleId == scheduleId)
            .OrderBy(x => x.WorkDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<EmployeeSchedule>>
        GetByWorkDateAsync(DateOnly workDate)
    {
        return await _context.EmployeeSchedules
            .Where(x => x.WorkDate == workDate)
            .OrderBy(x => x.EmployeeId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(
        Guid employeeId, Guid scheduleId, DateOnly workDate)
    {
        return await _context.EmployeeSchedules
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.ScheduleId == scheduleId &&
                x.WorkDate == workDate);
    }
}
