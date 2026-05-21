using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Application.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace EncuentraTuHogar.Application.Mappers;

public static class PropertyMapper
{
    public static PropertyDto ToDto(Property property) =>
        new PropertyDto(
            property.Id.Value.ToString(),
            property.Address.City,
            property.Address.Zone,
            property.Type,
            property.Price.Amount,
            property.ImageUrls.FirstOrDefault() ?? "",
            property.Views,
            0.0,
            property.IsLocalPriority);
            
    public static IEnumerable<PropertyDto> ToDto(IEnumerable<Property> properties) =>
        properties.Select(ToDto);
}
