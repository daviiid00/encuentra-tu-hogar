using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.DTOs;

public class SearchPropertiesRequest
{
    public string City { get; set; } = string.Empty;
    public PropertyType? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
