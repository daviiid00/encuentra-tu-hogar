using Microsoft.AspNetCore.Identity;

namespace EncuentraTuHogar.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsOwner { get; set; } = false;
}
