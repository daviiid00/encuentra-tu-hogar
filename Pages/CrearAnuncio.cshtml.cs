using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;
using EncuentraTuHogar.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Pages;

[Authorize]
public class CrearAnuncioModel : PageModel
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CrearAnuncioModel(IPropertyRepository propertyRepository, UserManager<ApplicationUser> userManager)
    {
        _propertyRepository = propertyRepository;
        _userManager = userManager;
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
        public PropertyType Type { get; set; }

        [Required]
        [Display(Name = "Tipo de Transacción")]
        public TransactionType Transaction { get; set; }

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

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var address = new Address(Input.City, Input.Zone, Input.Street, "00000");
        var price = new Price(Input.Price);
        var ownerId = UserId.From(System.Guid.Parse(user.Id));

        var propertyResult = Property.Create(address, Input.Type, Input.Transaction, price, ownerId, Input.Description);

        if (propertyResult is Result<Property>.Success success)
        {
            await _propertyRepository.AddAsync(success.Value);
            return RedirectToPage("/Dashboard");
        }
        else if (propertyResult is Result<Property>.Failure failure)
        {
            ModelState.AddModelError(string.Empty, failure.Error);
            return Page();
        }

        return Page();
    }
}
