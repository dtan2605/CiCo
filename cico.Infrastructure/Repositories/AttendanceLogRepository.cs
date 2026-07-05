using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class AttendanceLogRepository
    : BaseRepository<AttendanceLog>,
      IAttendanceLogRepository
{
    public AttendanceLogRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public override async Task<List<AttendanceLog>>
        GetAllAsync()
    {
        return await _context.AttendanceLogs
            .Include(x => x.Attendance)
            .Include(x => x.Device)
            .OrderByDescending(x => x.ScanTime)
            .ToListAsync();
    }

    public override async Task<AttendanceLog?>
        GetByIdAsync(Guid id)
    {
        return await _context.AttendanceLogs
            .Include(x => x.Attendance)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<AttendanceLog>>
        GetByAttendanceIdAsync(Guid attendanceId)
    {
        return await _context.AttendanceLogs
            .Include(x => x.Device)
            .Where(x =>
                x.AttendanceId == attendanceId)
            .OrderBy(x => x.ScanTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AttendanceLog>>
        GetByDeviceIdAsync(Guid deviceId)
    {
        return await _context.AttendanceLogs
            .Include(x => x.Attendance)
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.ScanTime)
            .ToListAsync();
    }
}
