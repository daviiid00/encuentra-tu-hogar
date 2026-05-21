using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Application.DTOs;

namespace EncuentraTuHogar.Application.Mappers;

public static class PropertyMapper
{
    public static PropertyDto ToDto(
        Property property,
        string? title = null,
        double? latitude = null,
        double? longitude = null,
        int stratum = 0,
        List<string>? services = null) => new PropertyDto(
            property.Id.Value.ToString(),
            title ?? $"Propiedad en {property.Address.City}",
            property.Description,
            property.Address.City,
            property.Address.Zone,
            property.Address.Street,
            property.Price.Amount,
            property.Price.Currency,
            property.Type.ToString(),
            property.Transaction.ToString(),
            property.Status.ToString(),
            stratum,
            latitude,
            longitude,
            property.Status == EncuentraTuHogar.Domain.ValueObjects.VerificationStatus.Verified,
            property.IsLocalPriority,
            property.ImageUrls,
            services ?? new List<string>(),
            property.OwnerId.Value.ToString(),
            property.Views,
            property.CreatedAt
        );

    public static IEnumerable<PropertyDto> ToDto(IEnumerable<Property> properties) =>
        properties.Select(p => ToDto(p));
}
