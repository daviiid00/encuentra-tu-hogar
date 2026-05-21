using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using EncuentraTuHogar.Infrastructure.Identity;

namespace EncuentraTuHogar.Application.Services;

// Spec: community.spec.md
public class CommunityService : ICommunityService
{
    private readonly ICommunityPostRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommunityService(
        ICommunityPostRepository repository,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        var posts = await _repository.GetAllAsync(onlyApproved: true);
        var dtos = new List<PostDto>();

        foreach (var post in posts)
        {
            var author = await _userManager.FindByIdAsync(post.AuthorId);
            dtos.Add(ToDto(post, author?.FullName ?? "Usuario"));
        }

        return dtos;
    }

    public async Task<Result<PostDto>> CreatePostAsync(CreatePostRequest request, string authorId)
    {
        if (!Enum.TryParse<CommunityPostCategory>(request.Category, out var category))
            return Result.Failure<PostDto>($"Categoría inválida: {request.Category}");

        var createResult = CommunityPost.Create(authorId, request.Title, request.Content, category);

        if (createResult is Result<CommunityPost>.Failure f)
            return Result.Failure<PostDto>(f.Error);

        var post = ((Result<CommunityPost>.Success)createResult).Value;
        await _repository.AddAsync(post);

        var author = await _userManager.FindByIdAsync(authorId);
        return Result.Success(ToDto(post, author?.FullName ?? "Usuario"));
    }

    public async Task<Result<bool>> ReportPostAsync(ReportPostRequest request, string reporterId)
    {
        var post = await _repository.GetByIdAsync(request.PostId);
        if (post == null)
            return Result.Failure<bool>("Publicación no encontrada");

        post.Report();
        await _repository.UpdateAsync(post);
        return Result.Success(true);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static PostDto ToDto(CommunityPost post, string authorName) => new(
        post.Id,
        post.AuthorId,
        authorName,
        post.Title,
        post.Content,
        post.Category.ToString(),
        post.IsReported,
        post.IsApproved,
        post.CreatedAt
    );
}
