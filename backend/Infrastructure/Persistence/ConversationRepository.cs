using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public ConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId)
    {
        return await _context.Conversations
            .Where(c => c.ParticipantAId == userId || c.ParticipantBId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Conversation?> FindBetweenUsersAsync(string userAId, string userBId, Guid? propertyId)
    {
        return await _context.Conversations.FirstOrDefaultAsync(c =>
            ((c.ParticipantAId == userAId && c.ParticipantBId == userBId) ||
             (c.ParticipantAId == userBId && c.ParticipantBId == userAId)) &&
            c.PropertyId == propertyId);
    }

    public async Task AddAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
    }

    public async Task AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
