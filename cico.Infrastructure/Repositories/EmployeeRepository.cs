using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;
public class EmployeeRepository
    : BaseRepository<Employee>,
      IEmployeeRepository
{
    public EmployeeRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<Employee?> GetDetailAsync(
        Guid id)
    {
        return await _context.Employees
            .Include(x => x.Department)
            .Include(x => x.Position)
            .Include(x => x.User)
            .Include(x => x.EmployeeSchedules)
                .ThenInclude(x => x.Schedule)
            .Include(x => x.BiometricProfiles)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Employee?> GetByCodeAsync(
        string employeeCode)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.EmployeeCode == employeeCode);
    }

    public async Task<Employee?> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.UserId == userId);
    }

    public async Task<List<Employee>>
        GetActiveEmployeesAsync()
    {
        return await _context.Employees
            .Where(x =>
                x.IsActive &&
                !x.IsDeleted)
            .ToListAsync();
    }

    public IQueryable<Employee> GetQueryable()
    {
        return _context.Employees.AsQueryable();
    }

    public async Task<Employee?> GetByEmailAsync(
        string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.Email == email
                && !x.IsDeleted);
    }

    public async Task<Employee?> GetByEmployeeCodeAsync(
        string employeeCode)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.EmployeeCode == employeeCode
                && !x.IsDeleted);
    }
    public async Task<List<Employee>>
        GetPagingAsync(
            int pageNumber,
            int pageSize,
            string? keyword)
    {
        var query = _context.Employees
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.FullName.Contains(keyword)
                || x.EmployeeCode.Contains(keyword)
                || x.Email.Contains(keyword));
        }

        return await query
            .Include(x => x.Department)
            .Include(x => x.Position)
            .OrderBy(x => x.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}