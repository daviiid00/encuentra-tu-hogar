using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/community")]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    // GET /api/community/posts  — public — spec: community.spec.md
    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _communityService.GetPostsAsync();
        return Ok(posts);
    }

    // POST /api/community/posts  — authenticated users only
    [Authorize]
    [HttpPost("posts")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(authorId)) return Unauthorized();

        var result = await _communityService.CreatePostAsync(request, authorId);
        return result switch
        {
            Result<PostDto>.Success s => CreatedAtAction(nameof(GetPosts), s.Value),
            Result<PostDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // POST /api/community/report  — spec: Users can report posts
    [Authorize]
    [HttpPost("report")]
    public async Task<IActionResult> ReportPost([FromBody] ReportPostRequest request)
    {
        var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(reporterId)) return Unauthorized();

        var result = await _communityService.ReportPostAsync(request, reporterId);
        return result switch
        {
            Result<bool>.Success => Ok(new { message = "Publicación reportada correctamente" }),
            Result<bool>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }
}
