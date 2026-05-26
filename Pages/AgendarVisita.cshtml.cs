using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Frontend.Services;
using EncuentraTuHogar.Application.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class AgendarVisitaModel : PageModel
{
    private readonly ApiClient _apiClient;

    public AgendarVisitaModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
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

        // Combina los dos campos en un DateTime para enviar al API
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

        if (Input.ScheduledDate < DateTime.Now)
        {
            ModelState.AddModelError("Input.ScheduledDate", "La fecha debe ser en el futuro.");
            return Page();
        }

        if (!Guid.TryParse(Input.PropertyId, out var propId))
        {
             ModelState.AddModelError("Input.PropertyId", "El ID de la propiedad no es válido.");
             return Page();
        }

        var request = new ScheduleVisitRequest(propId, Input.ScheduledDate);
        var visit = await _apiClient.CreateVisitAsync(request);

        if (visit == null)
        {
            ModelState.AddModelError(string.Empty, "Error al agendar la visita. Intente nuevamente.");
            return Page();
        }

        return RedirectToPage("/Dashboard");
    }
}
