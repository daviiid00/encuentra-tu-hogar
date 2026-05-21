namespace EncuentraTuHogar.Application.DTOs;

// Spec: community.spec.md

public record CreatePostRequest(
    string Title,
    string Content,
    string Category   // "Experience" | "Recommendation" | "Alert" | "Discussion" | "Support"
);

public record PostDto(
    Guid Id,
    string AuthorId,
    string AuthorName,
    string Title,
    string Content,
    string Category,
    bool IsReported,
    bool IsApproved,
    DateTime CreatedAt
);

public record ReportPostRequest(
    Guid PostId,
    string Reason
);
