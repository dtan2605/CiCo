using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace cico.Infrastructure.SignalR.Hubs;

[Authorize]
public class DashboardHub : Hub
{
    public async Task JoinDashboardGroup()
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, "Dashboard");
    }

    public async Task LeaveDashboardGroup()
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId, "Dashboard");
    }
}
