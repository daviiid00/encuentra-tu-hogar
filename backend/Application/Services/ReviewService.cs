using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using EncuentraTuHogar.Infrastructure.Identity;

namespace EncuentraTuHogar.Application.Services;

// Spec: reviews.spec.md
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewService(
        IReviewRepository reviewRepository,
        IVisitRepository visitRepository,
        UserManager<ApplicationUser> userManager)
    {
        _reviewRepository = reviewRepository;
        _visitRepository = visitRepository;
        _userManager = userManager;
    }

    public async Task<Result<ReviewDto>> CreateAsync(CreateReviewRequest request, string reviewerId)
    {
        // Spec: reviews.spec.md — Only completed interactions can be reviewed
        var visit = await _visitRepository.GetByIdAsync(request.VisitId);
        if (visit == null)
            return Result.Failure<ReviewDto>("Visita no encontrada");

        if (visit.Status != VisitStatus.Completed)
            return Result.Failure<ReviewDto>("Solo puedes reseñar visitas completadas");

        if (visit.VisitorId != reviewerId)
            return Result.Failure<ReviewDto>("Solo el visitante puede reseñar esta visita");

        // Check no duplicate review
        var existing = await _reviewRepository.GetByVisitIdAsync(request.VisitId);
        if (existing != null)
            return Result.Failure<ReviewDto>("Ya existe una reseña para esta visita");

        var createResult = Review.Create(request.VisitId, reviewerId, request.Rating, request.Comment);
        if (createResult is not Result<Review>.Success reviewSuccess)
            return Result.Failure<ReviewDto>(((Result<Review>.Failure)createResult).Error);

        var review = reviewSuccess.Value;
        await _reviewRepository.AddAsync(review);

        var reviewer = await _userManager.FindByIdAsync(reviewerId);
        return Result.Success(ToDto(review, reviewer?.FullName ?? "Usuario"));
    }

    public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId)
    {
        var reviews = await _reviewRepository.GetByReviewerIdAsync(userId);
        var user = await _userManager.FindByIdAsync(userId);
        var name = user?.FullName ?? "Usuario";
        return reviews.Select(r => ToDto(r, name));
    }

    private static ReviewDto ToDto(Review review, string reviewerName) => new(
        review.Id,
        review.VisitId,
        review.ReviewerId,
        reviewerName,
        review.Rating,
        review.Comment,
        review.IsApproved,
        review.CreatedAt
    );
}
