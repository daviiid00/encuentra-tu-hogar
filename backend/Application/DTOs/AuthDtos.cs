namespace EncuentraTuHogar.Application.DTOs;

// Spec: auth.spec.md

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string City,
    string Role,           // "Tenant" | "Landlord"
    bool IsLocalResident
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string Token,
    string UserId,
    string FullName,
    string Email,
    string Role,
    DateTime ExpiresAt
);

public record UserProfileDto(
    string Id,
    string FullName,
    string Email,
    string City,
    string Bio,
    string ProfileImageUrl,
    bool IsVerified,
    bool IsLocalResident,
    string Role,
    DateTime CreatedAt
);
