using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    // GET /api/chat/conversations  — spec: chat.spec.md
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var conversations = await _chatService.GetConversationsAsync(userId);
        return Ok(conversations);
    }

    // POST /api/chat/conversations  — start a new conversation
    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _chatService.StartConversationAsync(request, userId);
        return result switch
        {
            Result<ConversationDto>.Success s => Ok(s.Value),
            Result<ConversationDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // POST /api/chat/message  — spec: chat.spec.md
    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _chatService.SendMessageAsync(request, userId);
        return result switch
        {
            Result<MessageDto>.Success s => Ok(s.Value),
            Result<MessageDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // GET /api/chat/conversations/{id}/messages
    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetMessages(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var messages = await _chatService.GetMessagesAsync(id, userId);
        return Ok(messages);
    }

    private string? GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(ClaimTypes.Name);
}
