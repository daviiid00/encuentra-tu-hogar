using System.ComponentModel.DataAnnotations;

namespace EncuentraTuHogar.Application.DTOs;

// Spec: auth.spec.md

public record RegisterRequest(
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    string FullName,

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    string Email,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$", ErrorMessage = "La contraseña debe tener una mayúscula, un número y un carácter especial")]
    string Password,

    [Required(ErrorMessage = "La ciudad es obligatoria")]
    string City,

    [Required(ErrorMessage = "El rol es obligatorio")]
    string Role,           // "Tenant" | "Landlord"
    
    bool IsLocalResident
);

public record LoginRequest(
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    string Email,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
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
