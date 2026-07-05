using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class ScheduleRepository
    : BaseRepository<Schedule>,
      IScheduleRepository
{
    public ScheduleRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Schedule>>
        GetActiveSchedulesAsync()
    {
        return await _context.Schedules
            .Where(x => x.IsActive)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }
}
