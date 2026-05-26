using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.Domain.Entities;

// Spec: community.spec.md
public class CommunityPost : Entity
{
    public Guid Id { get; private set; }
    public string AuthorId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public CommunityPostCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsReported { get; private set; }
    public bool IsApproved { get; private set; }

    private CommunityPost() { }

    public static Result<CommunityPost> Create(
        string authorId,
        string title,
        string content,
        CommunityPostCategory category)
    {
        if (string.IsNullOrWhiteSpace(authorId))
            return Result.Failure<CommunityPost>("El autor es requerido");

        if (string.IsNullOrWhiteSpace(title) || title.Length < 5)
            return Result.Failure<CommunityPost>("El título debe tener al menos 5 caracteres");

        if (string.IsNullOrWhiteSpace(content) || content.Length < 20)
            return Result.Failure<CommunityPost>("El contenido debe tener al menos 20 caracteres");

        return Result.Success(new CommunityPost
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Title = title,
            Content = content,
            Category = category,
            CreatedAt = DateTime.UtcNow,
            IsReported = false,
            IsApproved = true
        });
    }

    public void Report() => IsReported = true;
    public void Approve() => IsApproved = true;
    public void Reject() => IsApproved = false;
}

public enum CommunityPostCategory
{
    Experience,
    Recommendation,
    Alert,
    Discussion,
    Support
}
