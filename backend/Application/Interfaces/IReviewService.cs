using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: reviews.spec.md
public interface IReviewService
{
    Task<Result<ReviewDto>> CreateAsync(CreateReviewRequest request, string reviewerId);
    Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId);
}
