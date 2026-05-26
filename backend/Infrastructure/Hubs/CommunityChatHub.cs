using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Infrastructure.Hubs;

public class CommunityChatHub : Hub
{
    public async Task JoinChannel(string channelName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
    }

    public async Task LeaveChannel(string channelName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
    }

    public async Task SendMessage(string channelName, string user, string message)
    {
        await Clients.Group(channelName).SendAsync("ReceiveMessage", user, message);
    }
}
