using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Shopizy.Infrastructure.Realtime.Hubs;

/// <summary>
/// SignalR hub broadcasting real-time metrics and sales streams to administrators.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminDashboardHub : Hub
{
    public const string AdminGroup = "Admins";

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        await base.OnConnectedAsync();
    }
}
