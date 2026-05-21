using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // POST /api/reviews  — spec: reviews.spec.md
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(reviewerId)) return Unauthorized();

        var result = await _reviewService.CreateAsync(request, reviewerId);
        return result switch
        {
            Result<ReviewDto>.Success s => CreatedAtAction(nameof(GetByUser), new { id = s.Value.ReviewerId }, s.Value),
            Result<ReviewDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // GET /api/reviews/user/{id}  — spec: reviews.spec.md
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetByUser(string id)
    {
        var reviews = await _reviewService.GetByUserIdAsync(id);
        return Ok(reviews);
    }
}
