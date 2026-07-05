using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class ReportRepository
    : IReportRepository
{
    private readonly CICODbContext _context;

    public ReportRepository(
        CICODbContext context)
    {
        _context = context;
    }

    public async Task<int>
        GetTotalEmployeeAsync()
    {
        return await _context.Employees
            .CountAsync(x => x.IsActive);
    }

    public async Task<int>
        GetPresentEmployeeAsync(DateOnly date)
    {
        return await _context.Attendances
            .CountAsync(x =>
                x.AttendanceDate == date &&
                x.CheckInTime != null);
    }

    public async Task<int>
        GetAbsentEmployeeAsync(DateOnly date)
    {
        var total =
            await GetTotalEmployeeAsync();
        var present =
            await GetPresentEmployeeAsync(date);
        return total - present;
    }

    public async Task<int>
        GetLateEmployeeAsync(DateOnly date)
    {
        return await _context.Attendances
            .CountAsync(x =>
                x.AttendanceDate == date &&
                x.Status == AttendanceStatus.Late);
    }

    public async Task<List<Attendance>>
        GetAttendanceByDateAsync(DateOnly date)
    {
        return await _context.Attendances
            .Include(x => x.Employee)
            .Where(x =>
                x.AttendanceDate == date)
            .OrderBy(x =>
                x.CheckInTime ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<Dictionary<DateOnly, int>>
        GetDailyCountsAsync(
            DateOnly from, DateOnly to)
    {
        return await _context.Attendances
            .Where(x =>
                x.AttendanceDate >= from &&
                x.AttendanceDate <= to &&
                x.CheckInTime != null)
            .GroupBy(x => x.AttendanceDate)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.Date,
                x => x.Count);
    }

    public async Task<Dictionary<DateOnly, int>>
        GetDailyLateCountsAsync(
            DateOnly from, DateOnly to)
    {
        return await _context.Attendances
            .Where(x =>
                x.AttendanceDate >= from &&
                x.AttendanceDate <= to &&
                x.Status == AttendanceStatus.Late)
            .GroupBy(x => x.AttendanceDate)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.Date,
                x => x.Count);
    }
}
