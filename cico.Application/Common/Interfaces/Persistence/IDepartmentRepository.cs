using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;
public interface IDepartmentRepository
    : IBaseRepository<Department>
{
    Task<Department?> GetDetailAsync(Guid id);

    Task<List<Department>> GetPagingAsync(
        int pageNumber,
        int pageSize,
        string? keyword);
}