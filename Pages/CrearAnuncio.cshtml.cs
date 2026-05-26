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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

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

        var result = await _apiClient.CreatePropertyAsync(request);

        if (result.Property != null)
        {
            return RedirectToPage("/Dashboard");
        }
        else
        {
            // Parse error if possible, or show generic
            string errorMsg = result.Error ?? "Hubo un error al crear la propiedad. Verifica que eres propietario y tienes sesión válida.";
            ModelState.AddModelError(string.Empty, errorMsg);
            return Page();
        }
    }
}
