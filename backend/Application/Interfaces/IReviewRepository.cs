using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: reviews.spec.md
public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId);
    Task<Review?> GetByVisitIdAsync(Guid visitId);
    Task AddAsync(Review review);
}
