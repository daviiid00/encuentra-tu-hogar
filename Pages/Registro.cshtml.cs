using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Frontend.Services;
using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Pages;

public class RegistroModel : PageModel
{
    private readonly ApiClient _apiClient;

    public RegistroModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; }

        [Display(Name = "¿Eres Propietario?")]
        public bool IsOwner { get; set; }
    }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var role = Input.IsOwner ? "Landlord" : "Tenant";
            var registerRequest = new RegisterRequest(Input.FullName, Input.Email, Input.Password, "", role, false);
            var result = await _apiClient.RegisterAsync(registerRequest);
            
            if (result.Response != null && !string.IsNullOrEmpty(result.Response.Token))
            {
                var authResponse = result.Response;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authResponse.UserId),
                    new Claim(ClaimTypes.Name, authResponse.FullName),
                    new Claim(ClaimTypes.Email, authResponse.Email)
                };

                if (!string.IsNullOrEmpty(authResponse.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, authResponse.Role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                authProperties.StoreTokens(new[]
                {
                    new AuthenticationToken { Name = "access_token", Value = authResponse.Token }
                });

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Error al registrar la cuenta.");
            }
        }

        return Page();
    }
}
