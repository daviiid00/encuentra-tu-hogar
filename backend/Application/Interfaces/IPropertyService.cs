using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.Interfaces;

// Spec: properties.spec.md
public interface IPropertyService
{
    Task<Result<PropertyDto>> CreateAsync(CreatePropertyRequest request, string ownerId);
    Task<Result<PropertyDto>> GetByIdAsync(string id);
    Task<IEnumerable<PropertyDto>> SearchAsync(PropertyFilterRequest filter);
    Task<Result<PropertyDto>> UpdateAsync(string id, UpdatePropertyRequest request, string requestingUserId);
    Task<Result<bool>> DeleteAsync(string id, string requestingUserId, bool isAdmin);
}
