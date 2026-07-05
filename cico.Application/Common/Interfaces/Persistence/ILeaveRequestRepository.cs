using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface ILeaveRequestRepository
    : IBaseRepository<LeaveRequest>
{
    Task<List<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<LeaveRequest>> GetPendingAsync();
}
