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
    private readonly IPropertyService _propertyService;

    public DashboardModel(ApiClient apiClient, IVisitRepository visitRepository, IPropertyService propertyService)
    {
        _apiClient = apiClient;
        _visitRepository = visitRepository;
        _propertyService = propertyService;
    }

    public IEnumerable<PropertyDto> MisPropiedades { get; set; } = new List<PropertyDto>();
    public IEnumerable<Visit> MisVisitasProgramadas { get; set; } = new List<Visit>();
    public IEnumerable<Visit> VisitasRecibidas { get; set; } = new List<Visit>();
    public string UserFullName { get; set; }
    public bool IsLandlord { get; set; }

    public async Task OnGetAsync()
    {
        UserFullName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario";
        IsLandlord = User.IsInRole("Landlord");
        
        if (IsLandlord)
        {
            MisPropiedades = await _apiClient.GetMyPropertiesAsync();
            var myPropIds = MisPropiedades.Where(p => Guid.TryParse(p.Id, out _)).Select(p => Guid.Parse(p.Id)).ToList();
            if (myPropIds.Any())
            {
                VisitasRecibidas = await _visitRepository.FindByPropertyIdsAsync(myPropIds);
            }
        }

        // (We leave visits using Repo for now since we haven't implemented Visit API client fully yet, 
        // to keep it within the scope of this step).
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            MisVisitasProgramadas = await _visitRepository.FindByVisitorIdAsync(userId);
        }
    }

    public async Task<IActionResult> OnPostDeletePropertyAsync(string propertyId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return RedirectToPage("/Login");

        var isAdmin = User.IsInRole("Admin");
        var result = await _propertyService.DeleteAsync(propertyId, userId, isAdmin);

        if (result is EncuentraTuHogar.Domain.Common.Result<bool>.Success)
        {
            TempData["SuccessMessage"] = "Propiedad eliminada correctamente.";
        }
        else
        {
            var error = result is EncuentraTuHogar.Domain.Common.Result<bool>.Failure f ? f.Error : "Error al eliminar";
            TempData["ErrorMessage"] = error;
        }

        return RedirectToPage();
    }
}
