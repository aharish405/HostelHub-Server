using Microsoft.AspNetCore.SignalR;

namespace HostelHub.Api.Hubs;

public class OccupancyHub : Hub
{
    public async Task JoinHostelGroup(string tenantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tenantId);
    }
}
