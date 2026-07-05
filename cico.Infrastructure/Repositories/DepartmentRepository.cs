using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class DepartmentRepository
    : BaseRepository<Department>,
      IDepartmentRepository
{
    public DepartmentRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<Department?>
        GetDetailAsync(Guid id)
    {
        return await _context.Departments
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Department>>
        GetPagingAsync(
            int pageNumber,
            int pageSize,
            string? keyword)
    {
        var query =
            _context.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(keyword)
                || x.Description.Contains(keyword));
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}