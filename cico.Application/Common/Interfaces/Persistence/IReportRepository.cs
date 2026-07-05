using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IReportRepository
{
    Task<int> GetTotalEmployeeAsync();
    Task<int> GetPresentEmployeeAsync(DateOnly date);
    Task<int> GetAbsentEmployeeAsync(DateOnly date);
    Task<int> GetLateEmployeeAsync(DateOnly date);
    Task<List<Attendance>> GetAttendanceByDateAsync(DateOnly date);
    Task<Dictionary<DateOnly, int>> GetDailyCountsAsync(
        DateOnly from, DateOnly to);

    Task<Dictionary<DateOnly, int>> GetDailyLateCountsAsync(
        DateOnly from, DateOnly to);
}
