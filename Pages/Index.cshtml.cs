using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Frontend.Services;

namespace EncuentraTuHogar.Pages;

public class IndexModel : PageModel
{
    private readonly ApiClient _apiClient;

    public IndexModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public List<PropertyDto> Properties { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Neighborhood { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PropertyType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PriceRange { get; set; }

    public async Task OnGetAsync()
    {
        var filter = new PropertyFilterRequest(null, null, null, null, null, null, true, null, null, null);
        filter = filter with { City = "Medellín" };

        if (!string.IsNullOrEmpty(Neighborhood))
        {
            filter = filter with { Neighborhood = Neighborhood };
        }

        if (!string.IsNullOrEmpty(PropertyType))
        {
            var typeString = PropertyType switch
            {
                "1" => "Apartment",
                "2" => "House",
                "3" => "Studio",
                _ => null
            };
            filter = filter with { Type = typeString };
        }

        if (!string.IsNullOrEmpty(PriceRange))
        {
            filter = PriceRange switch
            {
                "low" => filter with { MaxPrice = 1500000 },
                "medium" => filter with { MinPrice = 1500000, MaxPrice = 3000000 },
                "high" => filter with { MinPrice = 3000000 },
                _ => filter
            };
        }

        Properties = await _apiClient.GetPropertiesAsync(filter);
    }
}
