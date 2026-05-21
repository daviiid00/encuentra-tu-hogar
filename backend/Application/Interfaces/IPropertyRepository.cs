using System.Collections.Generic;
using System.Threading.Tasks;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.Interfaces;

public interface IPropertyRepository
{
    Task<IEnumerable<Property>> FindByFiltersAsync(SearchFilter filter);
    Task<Property?> FindByIdAsync(PropertyId id);
    Task AddAsync(Property property);
}
