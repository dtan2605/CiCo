using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;
public class AttendanceRepository
    : BaseRepository<Attendance>,
      IAttendanceRepository
{
    public AttendanceRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<Attendance?>
        GetAttendanceAsync(
            Guid employeeId,
            DateOnly date)
    {
        return await _context.Attendances
            .Include(x => x.Employee)
            .Include(x => x.EmployeeSchedule)
                .ThenInclude(x => x.Schedule)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeId &&
                x.AttendanceDate == date);
    }

    public async Task<List<Attendance>>
        GetAttendanceByEmployeeAsync(
            Guid employeeId)
    {
        return await _context.Attendances
            .Include(x => x.Employee)
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<List<Attendance>>
        GetAttendanceByDateAsync(
            DateOnly date)
    {
        return await _context.Attendances
            .Where(x =>
                x.AttendanceDate == date)
            .ToListAsync();
    }
}