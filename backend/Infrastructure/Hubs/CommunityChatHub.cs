using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Infrastructure.Hubs;

public class CommunityChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // Broadcast the message to all connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
