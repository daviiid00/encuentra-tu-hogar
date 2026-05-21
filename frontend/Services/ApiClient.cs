using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EncuentraTuHogar.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace EncuentraTuHogar.Frontend.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Extraer el token de la sesión (cookie) si existe
            var token = await context.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    // -- Auth --
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        return null;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        return null;
    }

    // -- Properties --
    public async Task<List<PropertyDto>> GetPropertiesAsync(string? city = null)
    {
        await SetAuthorizationHeaderAsync();
        var url = "/api/properties";
        if (!string.IsNullOrEmpty(city))
        {
            url += $"?city={Uri.EscapeDataString(city)}";
        }

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<PropertyDto>>() ?? new List<PropertyDto>();
        }
        return new List<PropertyDto>();
    }

    public async Task<List<PropertyDto>> GetMyPropertiesAsync()
    {
        await SetAuthorizationHeaderAsync();
        // El endpoint requiere ownerId, pero el usuario no siempre sabe su ID (se asume backend usa Claim si no hay,
        // pero nuestro backend dice `[FromQuery] string? ownerId`. Vamos a tener que ajustarlo o el backend ya lo hace).
        // En nuestro PropertyController: si ownerId no se manda, devuelve todo. Deberíamos mandar ownerId.
        // Pero espera, el ownerId lo tenemos en User.Claims.
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return new List<PropertyDto>();

        var url = $"/api/properties?ownerId={userId}";
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<PropertyDto>>() ?? new List<PropertyDto>();
        }
        return new List<PropertyDto>();
    }

    public async Task<PropertyDto?> CreatePropertyAsync(CreatePropertyRequest request)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/properties", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PropertyDto>();
        }
        return null;
    }
}
