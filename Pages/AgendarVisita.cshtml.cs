using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Infrastructure.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class AgendarVisitaModel : PageModel
{
    private readonly IVisitRepository _visitRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public AgendarVisitaModel(IVisitRepository visitRepository, UserManager<ApplicationUser> userManager)
    {
        _visitRepository = visitRepository;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "El ID de la propiedad es requerido para agendar.")]
        public string PropertyId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una fecha y hora.")]
        [Display(Name = "Fecha y Hora de Visita")]
        [DataType(DataType.DateTime)]
        public DateTime ScheduledDate { get; set; }
    }

    public void OnGet(string propertyId)
    {
        Input.PropertyId = propertyId;
        // Default to tomorrow
        Input.ScheduledDate = DateTime.Now.AddDays(1);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        if (Input.ScheduledDate < DateTime.Now)
        {
            ModelState.AddModelError("Input.ScheduledDate", "La fecha debe ser en el futuro.");
            return Page();
        }

        if (!Guid.TryParse(Input.PropertyId, out var propId))
        {
             // For testing purposes if they just clicked the generic "Agenda una visita" button
             propId = Guid.NewGuid(); // Fake ID just to show it works
        }

        var visit = Visit.Create(propId, user.Id, Input.ScheduledDate);
        await _visitRepository.AddAsync(visit);

        return RedirectToPage("/Dashboard");
    }
}
