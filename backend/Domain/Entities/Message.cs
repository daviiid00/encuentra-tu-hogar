using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.Domain.Entities;

// Spec: chat.spec.md
public class Message : Entity
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public string SenderId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }
    public bool IsRead { get; private set; }

    public Conversation Conversation { get; private set; } = null!;

    private Message() { }

    public static Result<Message> Create(Guid conversationId, string senderId, string content)
    {
        if (string.IsNullOrWhiteSpace(senderId))
            return Result.Failure<Message>("El remitente es requerido");

        if (string.IsNullOrWhiteSpace(content) || content.Length < 1)
            return Result.Failure<Message>("El mensaje no puede estar vacío");

        if (content.Length > 2000)
            return Result.Failure<Message>("El mensaje no puede superar 2000 caracteres");

        return Result.Success(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
    }

    public void MarkAsRead() => IsRead = true;
}
