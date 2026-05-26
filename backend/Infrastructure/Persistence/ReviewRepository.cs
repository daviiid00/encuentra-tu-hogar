using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId)
    {
        return await _context.Reviews
            .Where(r => r.ReviewerId == reviewerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByVisitIdAsync(Guid visitId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.VisitId == visitId);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }
}
