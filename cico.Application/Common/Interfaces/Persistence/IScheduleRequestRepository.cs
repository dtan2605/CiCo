using cico.Domain.Entities;
using cico.Domain.Enums;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IScheduleRequestRepository
{
    Task<List<ScheduleRequest>> GetPagingAsync(int pageNumber, int pageSize, Guid? employeeId, ScheduleRequestStatus? status);
    Task<ScheduleRequest?> GetByIdAsync(Guid id);
    Task AddAsync(ScheduleRequest request);
    Task UpdateAsync(ScheduleRequest request);
}
