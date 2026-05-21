using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using EncuentraTuHogar.Infrastructure.Identity;

namespace EncuentraTuHogar.Application.Services;

// Spec: chat.spec.md
public class ChatService : IChatService
{
    private readonly IConversationRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatService(
        IConversationRepository repository,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ConversationDto>> GetConversationsAsync(string userId)
    {
        var conversations = await _repository.GetByUserIdAsync(userId);
        var dtos = new List<ConversationDto>();

        foreach (var conv in conversations)
        {
            var userA = await _userManager.FindByIdAsync(conv.ParticipantAId);
            var userB = await _userManager.FindByIdAsync(conv.ParticipantBId);
            var messages = await _repository.GetMessagesAsync(conv.Id);
            var lastMsg = messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content;

            dtos.Add(ToDto(conv, userA?.FullName ?? "Usuario", userB?.FullName ?? "Usuario", lastMsg));
        }

        return dtos;
    }

    public async Task<Result<ConversationDto>> StartConversationAsync(StartConversationRequest request, string initiatorId)
    {
        var recipient = await _userManager.FindByIdAsync(request.RecipientId);
        if (recipient == null)
            return Result.Failure<ConversationDto>("El destinatario no existe");

        // Avoid duplicate conversations for same property context
        var existing = await _repository.FindBetweenUsersAsync(initiatorId, request.RecipientId, request.PropertyId);
        if (existing != null)
        {
            var userA2 = await _userManager.FindByIdAsync(existing.ParticipantAId);
            var userB2 = await _userManager.FindByIdAsync(existing.ParticipantBId);
            return Result.Success(ToDto(existing, userA2?.FullName ?? "Usuario", userB2?.FullName ?? "Usuario", null));
        }

        var createResult = Conversation.Create(initiatorId, request.RecipientId, request.PropertyId);
        if (createResult is Result<Conversation>.Failure f)
            return Result.Failure<ConversationDto>(f.Error);

        var conversation = ((Result<Conversation>.Success)createResult).Value;
        await _repository.AddAsync(conversation);

        var initiator = await _userManager.FindByIdAsync(initiatorId);
        return Result.Success(ToDto(conversation, initiator?.FullName ?? "Usuario", recipient.FullName, null));
    }

    public async Task<Result<MessageDto>> SendMessageAsync(SendMessageRequest request, string senderId)
    {
        var conversation = await _repository.GetByIdAsync(request.ConversationId);
        if (conversation == null)
            return Result.Failure<MessageDto>("Conversación no encontrada");

        // Spec: chat.spec.md — Only participants can send messages
        var isParticipant = conversation.ParticipantAId == senderId || conversation.ParticipantBId == senderId;
        if (!isParticipant)
            return Result.Failure<MessageDto>("No eres participante de esta conversación");

        var createResult = Message.Create(request.ConversationId, senderId, request.Content);
        if (createResult is Result<Message>.Failure f)
            return Result.Failure<MessageDto>(f.Error);

        var message = ((Result<Message>.Success)createResult).Value;
        await _repository.AddMessageAsync(message);

        var sender = await _userManager.FindByIdAsync(senderId);
        return Result.Success(new MessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            sender?.FullName ?? "Usuario",
            message.Content,
            message.IsRead,
            message.SentAt
        ));
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid conversationId, string requestingUserId)
    {
        var conversation = await _repository.GetByIdAsync(conversationId);
        if (conversation == null) return Enumerable.Empty<MessageDto>();

        var isParticipant = conversation.ParticipantAId == requestingUserId || conversation.ParticipantBId == requestingUserId;
        if (!isParticipant) return Enumerable.Empty<MessageDto>();

        var messages = await _repository.GetMessagesAsync(conversationId);
        var dtos = new List<MessageDto>();

        foreach (var msg in messages.OrderBy(m => m.SentAt))
        {
            var sender = await _userManager.FindByIdAsync(msg.SenderId);
            dtos.Add(new MessageDto(
                msg.Id, msg.ConversationId, msg.SenderId,
                sender?.FullName ?? "Usuario", msg.Content, msg.IsRead, msg.SentAt
            ));
        }

        return dtos;
    }

    private static ConversationDto ToDto(Conversation conv, string nameA, string nameB, string? lastMsg) => new(
        conv.Id,
        conv.ParticipantAId, nameA,
        conv.ParticipantBId, nameB,
        conv.PropertyId,
        lastMsg,
        conv.CreatedAt
    );
}
