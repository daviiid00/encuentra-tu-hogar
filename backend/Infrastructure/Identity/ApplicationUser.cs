using Microsoft.AspNetCore.Identity;

namespace EncuentraTuHogar.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsOwner { get; set; } = false;

    // Campos adicionales (spec: auth.spec.md, roles.md)
    public string City { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public bool IsLocalResident { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
