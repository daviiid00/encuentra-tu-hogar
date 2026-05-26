using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: chat.spec.md
public interface IConversationRepository
{
    Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<Conversation?> FindBetweenUsersAsync(string userAId, string userBId, Guid? propertyId);
    Task AddAsync(Conversation conversation);
    Task AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId);
}
