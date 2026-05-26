using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.Interfaces;

// Extended repository with Update and Delete
public interface IPropertyRepository
{
    Task<IEnumerable<Property>> FindByFiltersAsync(SearchFilter filter);
    Task<Property?> FindByIdAsync(PropertyId id);
    Task AddAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(Property property);
}
