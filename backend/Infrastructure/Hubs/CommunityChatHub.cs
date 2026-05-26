using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Infrastructure.Hubs;

public class ChatMessageDto
{
    public string User { get; set; }
    public string Message { get; set; }
    public string Timestamp { get; set; }
}

public class CommunityChatHub : Hub
{
    // Historial temporal en memoria: guarda hasta 50 mensajes por canal
    private static readonly ConcurrentDictionary<string, List<ChatMessageDto>> _channelHistory = new();
    private const int MaxMessages = 50;

    public async Task JoinChannel(string channelName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
        
        // Enviar historial al usuario que se acaba de conectar a este canal
        if (_channelHistory.TryGetValue(channelName, out var history))
        {
            // Create a copy to avoid threading issues during serialization
            List<ChatMessageDto> historyCopy;
            lock (history)
            {
                historyCopy = new List<ChatMessageDto>(history);
            }
            await Clients.Caller.SendAsync("LoadHistory", historyCopy);
        }
    }

    public async Task LeaveChannel(string channelName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
    }

    public async Task SendMessage(string channelName, string user, string message)
    {
        // En Colombia (UTC-5)
        var colombiaTime = DateTime.UtcNow.AddHours(-5);
        var timeString = colombiaTime.ToString("hh:mm tt");

        var chatMsg = new ChatMessageDto
        {
            User = user,
            Message = message,
            Timestamp = timeString
        };

        var history = _channelHistory.GetOrAdd(channelName, _ => new List<ChatMessageDto>());
        lock (history)
        {
            history.Add(chatMsg);
            if (history.Count > MaxMessages)
            {
                history.RemoveAt(0);
            }
        }

        await Clients.Group(channelName).SendAsync("ReceiveMessage", user, message, timeString);
    }
}
