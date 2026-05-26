namespace EncuentraTuHogar.Application.DTOs;

// Spec: chat.spec.md

public record SendMessageRequest(
    Guid ConversationId,
    string Content
);

public record StartConversationRequest(
    string RecipientId,
    Guid? PropertyId
);

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    string SenderId,
    string SenderName,
    string Content,
    bool IsRead,
    DateTime SentAt
);

public record ConversationDto(
    Guid Id,
    string ParticipantAId,
    string ParticipantAName,
    string ParticipantBId,
    string ParticipantBName,
    Guid? PropertyId,
    string? LastMessage,
    DateTime CreatedAt
);
