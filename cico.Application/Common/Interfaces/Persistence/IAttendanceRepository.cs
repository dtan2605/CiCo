using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;
public interface IAttendanceRepository
    : IBaseRepository<Attendance>
{
    Task<Attendance?> GetAttendanceAsync(
        Guid employeeId,
        DateOnly date);

    Task<List<Attendance>>
        GetAttendanceByEmployeeAsync(
            Guid employeeId);

    Task<List<Attendance>>
        GetAttendanceByDateAsync(
            DateOnly date);
}