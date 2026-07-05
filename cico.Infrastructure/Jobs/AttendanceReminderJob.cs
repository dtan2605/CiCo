using cico.Domain.Enums;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Jobs;

public class AttendanceReminderJob
{
    private readonly CICODbContext _context;

    public AttendanceReminderJob(CICODbContext context)
    {
        _context = context;
    }

    public async Task RunAsync()
    {
        var today = DateOnly.FromDateTime(
            DateTime.UtcNow);

        var employeesWithoutCheckIn =
            await _context.Employees
                .Where(e => e.IsActive)
                .Where(e =>
                    !_context.Attendances.Any(a =>
                        a.EmployeeId == e.Id &&
                        a.AttendanceDate == today))
                .Where(e =>
                    !_context.LeaveRequests.Any(l =>
                        l.EmployeeId == e.Id &&
                        l.Status == LeaveStatus.Approved &&
                        l.FromDate <= today &&
                        l.ToDate >= today))
                .Select(e => new
                {
                    e.Id,
                    e.FullName
                })
                .ToListAsync();

        foreach (var employee in
            employeesWithoutCheckIn)
        {
            var notification = new
                Domain.Entities.Notification
                {
                    EmployeeId = employee.Id,
                    Title = "Check-In Reminder",
                    Content =
                        $"You haven't checked in today ({today:yyyy-MM-dd}). Please check in now.",
                    Type = NotificationType.Warning,
                    IsRead = false
                };

            _context.Notifications.Add(
                notification);
        }

        await _context.SaveChangesAsync();
    }
}
