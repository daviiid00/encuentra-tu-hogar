using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.Domain.Entities;

// Spec: chat.spec.md
public class Conversation : Entity
{
    public Guid Id { get; private set; }
    public string ParticipantAId { get; private set; } = string.Empty;
    public string ParticipantBId { get; private set; } = string.Empty;
    public Guid? PropertyId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    private readonly List<Message> _messages = new();

    private Conversation() { }

    public static Result<Conversation> Create(
        string participantAId,
        string participantBId,
        Guid? propertyId = null)
    {
        if (string.IsNullOrWhiteSpace(participantAId) || string.IsNullOrWhiteSpace(participantBId))
            return Result.Failure<Conversation>("Ambos participantes son requeridos");

        if (participantAId == participantBId)
            return Result.Failure<Conversation>("Un usuario no puede conversar consigo mismo");

        return Result.Success(new Conversation
        {
            Id = Guid.NewGuid(),
            ParticipantAId = participantAId,
            ParticipantBId = participantBId,
            PropertyId = propertyId,
            CreatedAt = DateTime.UtcNow
        });
    }
}
