using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IEmployeeScheduleRepository
    : IBaseRepository<EmployeeSchedule>
{
    Task<IReadOnlyList<EmployeeSchedule>>
        GetByEmployeeIdAsync(Guid employeeId);

    Task<IReadOnlyList<EmployeeSchedule>>
        GetByScheduleIdAsync(Guid scheduleId);

    Task<IReadOnlyList<EmployeeSchedule>>
        GetByWorkDateAsync(DateOnly workDate);

    Task<bool> ExistsAsync(
        Guid employeeId, Guid scheduleId, DateOnly workDate);
}
