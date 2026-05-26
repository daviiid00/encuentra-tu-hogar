using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Infrastructure.Identity;

namespace EncuentraTuHogar.Application.Services;

// Spec: auth.spec.md
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        // Validate role
        var allowedRoles = new[] { "Tenant", "Landlord" };
        if (!allowedRoles.Contains(request.Role))
            return Result.Failure<AuthResponse>("Rol inválido. Use 'Tenant' o 'Landlord'");

        // Check unique email (spec: auth.spec.md — Email must be unique)
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            return Result.Failure<AuthResponse>("El correo ya está registrado");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            City = request.City,
            IsOwner = request.Role == "Landlord",
            IsLocalResident = request.IsLocalResident,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(errors);
        }

        // Ensure role exists and assign
        if (!await _roleManager.RoleExistsAsync(request.Role))
            await _roleManager.CreateAsync(new IdentityRole(request.Role));

        await _userManager.AddToRoleAsync(user, request.Role);

        return await BuildAuthResponseAsync(user, request.Role);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Failure<AuthResponse>("Credenciales inválidas");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return Result.Failure<AuthResponse>("Credenciales inválidas");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Tenant";

        return await BuildAuthResponseAsync(user, role);
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<UserProfileDto>("Usuario no encontrado");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Tenant";

        return Result.Success(new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.City,
            user.Bio,
            user.ProfileImageUrl,
            user.IsVerified,
            user.IsLocalResident,
            role,
            user.CreatedAt
        ));
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private Task<Result<AuthResponse>> BuildAuthResponseAsync(ApplicationUser user, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"]
            ?? throw new InvalidOperationException("JWT Secret no configurado");
        var issuer = jwtSettings["Issuer"] ?? "EncuentraTuHogar";
        var audience = jwtSettings["Audience"] ?? "EncuentraTuHogarUsers";
        var hours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(hours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, role),
            new Claim("role", role),
            new Claim("isVerified", user.IsVerified.ToString()),
            new Claim("isLocalResident", user.IsLocalResident.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult<Result<AuthResponse>>(Result.Success(new AuthResponse(
            tokenString,
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            role,
            expiry
        )));
    }
}
