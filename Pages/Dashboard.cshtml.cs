using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EncuentraTuHogar.Frontend.Services;
using EncuentraTuHogar.Application.DTOs;
using System.Security.Claims;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApiClient _apiClient;
    private readonly IVisitRepository _visitRepository;

    public DashboardModel(ApiClient apiClient, IVisitRepository visitRepository)
    {
        _apiClient = apiClient;
        _visitRepository = visitRepository;
    }

    public IEnumerable<PropertyDto> MisPropiedades { get; set; } = new List<PropertyDto>();
    public IEnumerable<Visit> MisVisitasProgramadas { get; set; } = new List<Visit>();
    public string UserFullName { get; set; }

    public async Task OnGetAsync()
    {
        UserFullName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";
        
        // As we are acting as Frontend, we call the API
        // Currently the token should tell if it's owner. We check if they have Landlord role or IsOwner claim
        var isOwner = User.IsInRole("Landlord") || true; // Simplification, if they call GetMyProperties it will return empty if not an owner.
        
        MisPropiedades = await _apiClient.GetMyPropertiesAsync();

        // (We leave visits using Repo for now since we haven't implemented Visit API client fully yet, 
        // to keep it within the scope of this step).
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            MisVisitasProgramadas = await _visitRepository.FindByVisitorIdAsync(userId);
        }
    }
}
