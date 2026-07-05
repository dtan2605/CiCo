using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;
public interface IEmployeeRepository
    : IBaseRepository<Employee>
{
    Task<Employee?> GetDetailAsync(Guid id);

    Task<Employee?> GetByCodeAsync(
        string employeeCode);

    Task<Employee?> GetByUserIdAsync(
        Guid userId);

    Task<List<Employee>> GetActiveEmployeesAsync();
    IQueryable<Employee> GetQueryable();

    Task<Employee?> GetByEmailAsync(string email);

    Task<Employee?> GetByEmployeeCodeAsync(
        string employeeCode);
    
    Task<List<Employee>> GetPagingAsync(
        int pageNumber,
        int pageSize,
        string? keyword);
}