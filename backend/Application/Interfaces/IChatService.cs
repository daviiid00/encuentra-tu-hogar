using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: chat.spec.md
public interface IChatService
{
    Task<IEnumerable<ConversationDto>> GetConversationsAsync(string userId);
    Task<Result<ConversationDto>> StartConversationAsync(StartConversationRequest request, string initiatorId);
    Task<Result<MessageDto>> SendMessageAsync(SendMessageRequest request, string senderId);
    Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId, string requestingUserId);
}
