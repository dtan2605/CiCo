using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IAttendanceLogRepository
    : IBaseRepository<AttendanceLog>
{
    Task<IReadOnlyList<AttendanceLog>>
        GetByAttendanceIdAsync(Guid attendanceId);

    Task<IReadOnlyList<AttendanceLog>>
        GetByDeviceIdAsync(Guid deviceId);
}
