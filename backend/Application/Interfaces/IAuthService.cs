using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: auth.spec.md
public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<UserProfileDto>> GetProfileAsync(string userId);
}
