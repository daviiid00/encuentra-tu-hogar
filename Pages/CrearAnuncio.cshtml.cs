using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;

namespace EncuentraTuHogar.Pages;

[Authorize(Roles = "Landlord")]
public class CrearAnuncioModel : PageModel
{
    private readonly IPropertyService _propertyService;

    public CrearAnuncioModel(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "Ciudad")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Zona/Barrio")]
        public string Zone { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Tipo de Propiedad")]
        public EncuentraTuHogar.Domain.ValueObjects.PropertyType Type { get; set; }

        [Required]
        [Display(Name = "Tipo de Transacción")]
        public EncuentraTuHogar.Domain.ValueObjects.TransactionType Transaction { get; set; }

        [Required]
        [Display(Name = "Precio ($)")]
        public decimal Price { get; set; }

        [Required]
        [MinLength(20)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }

    public void OnGet() { }

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

        var request = new CreatePropertyRequest(
            $"Propiedad en Medellín",
            Input.Description,
            "Medellín",
            Input.Zone,
            Input.Street,
            "00000",
            Input.Price,
            Input.Type.ToString(),
            Input.Transaction.ToString(),
            3,
            null,
            null,
            new List<string>(),
            new List<string>()
        );

        // Llamar directamente al servicio — sin HTTP interno
        var result = await _propertyService.CreateAsync(request, userId);

        if (result is EncuentraTuHogar.Domain.Common.Result<EncuentraTuHogar.Application.DTOs.PropertyDto>.Success)
            return RedirectToPage("/Dashboard");

        var error = result is EncuentraTuHogar.Domain.Common.Result<EncuentraTuHogar.Application.DTOs.PropertyDto>.Failure f
            ? f.Error : "Error al crear la propiedad.";
        ModelState.AddModelError(string.Empty, error);
        return Page();
    }
}
