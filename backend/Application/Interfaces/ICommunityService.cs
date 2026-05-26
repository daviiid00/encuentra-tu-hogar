using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: community.spec.md
public interface ICommunityService
{
    Task<IEnumerable<PostDto>> GetPostsAsync();
    Task<Result<PostDto>> CreatePostAsync(CreatePostRequest request, string authorId);
    Task<Result<bool>> ReportPostAsync(ReportPostRequest request, string reporterId);
}
