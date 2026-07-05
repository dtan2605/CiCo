using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IScheduleRepository
    : IBaseRepository<Schedule>
{
    Task<IReadOnlyList<Schedule>>
        GetActiveSchedulesAsync();
}