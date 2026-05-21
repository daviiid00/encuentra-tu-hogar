using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class CommunityPostRepository : ICommunityPostRepository
{
    private readonly AppDbContext _context;

    public CommunityPostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommunityPost>> GetAllAsync(bool onlyApproved = true)
    {
        var query = _context.CommunityPosts.AsQueryable();
        if (onlyApproved)
            query = query.Where(p => p.IsApproved && !p.IsReported);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<CommunityPost?> GetByIdAsync(Guid id)
    {
        return await _context.CommunityPosts.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(CommunityPost post)
    {
        await _context.CommunityPosts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommunityPost post)
    {
        _context.CommunityPosts.Update(post);
        await _context.SaveChangesAsync();
    }
}
