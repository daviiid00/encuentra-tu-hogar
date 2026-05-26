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

public class LoginModel : PageModel
{
    private readonly ApiClient _apiClient;

    public LoginModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var result = await _apiClient.LoginAsync(new LoginRequest(Input.Email, Input.Password));
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
                    IsPersistent = Input.RememberMe
                };

                // Store the JWT token in the cookie so ApiClient can use it later
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
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Intento de inicio de sesión no válido.");
                return Page();
            }
        }

        return Page();
    }
}
