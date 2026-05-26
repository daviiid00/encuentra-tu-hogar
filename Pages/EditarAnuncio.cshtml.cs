using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;

namespace EncuentraTuHogar.Pages;

[Authorize(Roles = "Landlord")]
public class EditarAnuncioModel : PageModel
{
    private readonly IPropertyService _propertyService;

    public EditarAnuncioModel(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string PropertyId { get; set; }

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

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToPage("/Dashboard");
        
        PropertyId = id;
        var result = await _propertyService.GetByIdAsync(id);
        
        if (result is EncuentraTuHogar.Domain.Common.Result<EncuentraTuHogar.Application.DTOs.PropertyDto>.Success s)
        {
            var prop = s.Value;
            
            // Validate owner
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (prop.OwnerId != userId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "No tienes permiso para editar esta propiedad.";
                return RedirectToPage("/Dashboard");
            }

            Input.City = prop.City;
            Input.Zone = prop.Neighborhood;
            Input.Street = prop.Street;
            Input.Price = prop.Price;
            Input.Description = prop.Description;
            
            if (Enum.TryParse<EncuentraTuHogar.Domain.ValueObjects.PropertyType>(prop.Type, out var pType))
                Input.Type = pType;
                
            if (Enum.TryParse<EncuentraTuHogar.Domain.ValueObjects.TransactionType>(prop.Transaction, out var tType))
                Input.Transaction = tType;
                
            return Page();
        }
        
        return RedirectToPage("/Dashboard");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "No se pudo identificar tu usuario. Intenta iniciar sesión de nuevo.");
            return Page();
        }

        var request = new UpdatePropertyRequest(
            null,
            Input.Description,
            Input.Price,
            null,
            null,
            null,
            null
        );

        var result = await _propertyService.UpdateAsync(PropertyId, request, userId);

        if (result is EncuentraTuHogar.Domain.Common.Result<EncuentraTuHogar.Application.DTOs.PropertyDto>.Success)
        {
            TempData["SuccessMessage"] = "Propiedad actualizada correctamente.";
            return RedirectToPage("/Dashboard");
        }

        var error = result is EncuentraTuHogar.Domain.Common.Result<EncuentraTuHogar.Application.DTOs.PropertyDto>.Failure f
            ? f.Error : "Error al actualizar la propiedad.";
        ModelState.AddModelError(string.Empty, error);
        return Page();
    }
}
