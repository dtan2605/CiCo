using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Repositories;

public class NotificationRepository
    : BaseRepository<Notification>,
      INotificationRepository
{
    public NotificationRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Notification>>
        GetUnreadAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(x =>
                x.EmployeeId == userId &&
                !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
