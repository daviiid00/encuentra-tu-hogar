using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class AgendarVisitaModel : PageModel
{
    private readonly IVisitRepository _visitRepository;

    public AgendarVisitaModel(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "El ID de la propiedad es requerido para agendar.")]
        public string PropertyId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la fecha de la visita.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateOnly VisitDate { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la hora de la visita.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora")]
        public TimeOnly VisitTime { get; set; }

        // Combina los dos campos en un DateTime para guardar
        public DateTime ScheduledDate =>
            VisitDate.ToDateTime(VisitTime);
    }

    public void OnGet(string propertyId)
    {
        Input.PropertyId = propertyId;
        // Default: mañana a las 10:00 am (sin segundos)
        var manana = DateTime.Now.AddDays(1);
        Input.VisitDate = DateOnly.FromDateTime(manana);
        Input.VisitTime = new TimeOnly(10, 0);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Obtener el ID del usuario directamente del claim de la cookie
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "No se pudo identificar tu usuario. Intenta iniciar sesión de nuevo.");
            return Page();
        }

        if (Input.ScheduledDate < DateTime.Now)
        {
            ModelState.AddModelError("Input.VisitDate", "La fecha debe ser en el futuro.");
            return Page();
        }

        if (!Guid.TryParse(Input.PropertyId, out var propId))
        {
            ModelState.AddModelError("Input.PropertyId", "El ID de la propiedad no es válido.");
            return Page();
        }

        // Crear la visita directamente — sin HTTP interno
        var visit = Visit.Create(propId, userId, Input.ScheduledDate);
        await _visitRepository.AddAsync(visit);

        return RedirectToPage("/Dashboard");
    }
}
