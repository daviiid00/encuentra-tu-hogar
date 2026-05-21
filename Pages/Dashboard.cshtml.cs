using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Infrastructure.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(
        IPropertyRepository propertyRepository, 
        IVisitRepository visitRepository, 
        UserManager<ApplicationUser> userManager)
    {
        _propertyRepository = propertyRepository;
        _visitRepository = visitRepository;
        _userManager = userManager;
    }

    public IEnumerable<Property> MisPropiedades { get; set; } = new List<Property>();
    public IEnumerable<Visit> MisVisitasProgramadas { get; set; } = new List<Visit>();
    public string UserFullName { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            UserFullName = user.FullName;
            
            // Si es propietario, cargamos sus propiedades
            if (user.IsOwner)
            {
                var filter = new SearchFilter(); // Un filtro vacío para usar LINQ despues. En un caso real usaríamos un metodo especifico del repositorio.
                var todas = await _propertyRepository.FindByFiltersAsync(filter);
                MisPropiedades = System.Linq.Enumerable.Where(todas, p => p.OwnerId.Value.ToString() == user.Id);
            }
            
            MisVisitasProgramadas = await _visitRepository.FindByVisitorIdAsync(user.Id);
        }
    }
}
