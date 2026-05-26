namespace EncuentraTuHogar.Application.DTOs;

// Spec: reviews.spec.md

public record CreateReviewRequest(
    Guid VisitId,
    int Rating,     // 1-5
    string Comment
);

public record ReviewDto(
    Guid Id,
    Guid VisitId,
    string ReviewerId,
    string ReviewerName,
    int Rating,
    string Comment,
    bool IsApproved,
    DateTime CreatedAt
);
