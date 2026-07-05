using Microsoft.AspNetCore.SignalR;
using cico.Infrastructure.SignalR.Hubs;
using cico.Application.Interfaces;

namespace cico.Infrastructure.SignalR;

public class NotificationService
    : INotificationService
{
    private readonly IHubContext<NotificationHub>
        _notificationHub;

    public NotificationService(
        IHubContext<NotificationHub> notificationHub)
    {
        _notificationHub = notificationHub;
    }

    public async Task SendNotificationAsync(
        Guid userId,
        string title,
        string content,
        string type)
    {
        await _notificationHub.Clients
            .Group(userId.ToString())
            .SendAsync("ReceiveNotification", new
            {
                Title = title,
                Content = content,
                Type = type,
                CreatedAt = DateTime.UtcNow
            });
    }
}
