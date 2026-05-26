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
