using Microsoft.AspNetCore.Mvc;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/register  — spec: auth.spec.md
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return result switch
        {
            Result<AuthResponse>.Success s => Ok(s.Value),
            Result<AuthResponse>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // POST /api/auth/login  — spec: auth.spec.md
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return result switch
        {
            Result<AuthResponse>.Success s => Ok(s.Value),
            Result<AuthResponse>.Failure f => Unauthorized(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }
}
