using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Jobs;

public class NotificationCleanupJob
{
    private readonly CICODbContext _context;

    public NotificationCleanupJob(CICODbContext context)
    {
        _context = context;
    }

    public async Task RunAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);

        var oldNotifications =
            await _context.Notifications
                .Where(n => n.CreatedAt < cutoff)
                .ToListAsync();

        if (oldNotifications.Count == 0)
            return;

        _context.Notifications.RemoveRange(
            oldNotifications);

        await _context.SaveChangesAsync();
    }
}
