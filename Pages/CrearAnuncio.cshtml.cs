using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EncuentraTuHogar.Frontend.Services;
using EncuentraTuHogar.Application.DTOs;
using System.Security.Claims;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class CrearAnuncioModel : PageModel
{
    private readonly ApiClient _apiClient;

    public CrearAnuncioModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
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
        public string Type { get; set; } // Modificado a string para facilitar UI binding

        [Required]
        [Display(Name = "Tipo de Transacción")]
        public string Transaction { get; set; } // Modificado a string para UI binding

        [Required]
        [Display(Name = "Precio ($)")]
        public decimal Price { get; set; }

        [Required]
        [MinLength(20)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var request = new CreatePropertyRequest(
            $"Propiedad en {Input.City}",
            Input.Description,
            Input.City,
            Input.Zone,
            Input.Street,
            Input.Price,
            "COP",
            Input.Type,
            Input.Transaction,
            3,
            null,
            null,
            new List<string>(),
            new List<string>()
        );

        var propertyDto = await _apiClient.CreatePropertyAsync(request);

        if (propertyDto != null)
        {
            return RedirectToPage("/Dashboard");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Hubo un error al crear la propiedad. Verifica que eres propietario y tienes sesión válida.");
            return Page();
        }
    }
}
