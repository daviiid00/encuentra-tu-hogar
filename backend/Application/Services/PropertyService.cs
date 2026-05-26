using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Application.Mappers;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.Services;

// Spec: properties.spec.md
public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _repository;

    public PropertyService(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PropertyDto>> CreateAsync(CreatePropertyRequest request, string ownerId)
    {
        if (!Enum.TryParse<PropertyType>(request.Type, out var propType))
            return Result.Failure<PropertyDto>($"Tipo de propiedad inválido: {request.Type}");

        if (!Enum.TryParse<TransactionType>(request.Transaction, out var txType))
            return Result.Failure<PropertyDto>($"Tipo de transacción inválido: {request.Transaction}");

        Address address;
        try
        {
            address = new Address(request.City, request.Neighborhood, request.Street, request.PostalCode);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<PropertyDto>(ex.Message);
        }

        Price price;
        try
        {
            price = new Price(request.Price);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<PropertyDto>(ex.Message);
        }

        var ownerUserId = UserId.From(Guid.Parse(ownerId));

        // Fallback title from address if empty
        var title = string.IsNullOrWhiteSpace(request.Title)
            ? $"Propiedad en {request.City}"
            : request.Title;

        var createResult = Property.Create(address, propType, txType, price, ownerUserId, request.Description);

        if (createResult is Result<Property>.Failure f)
            return Result.Failure<PropertyDto>(f.Error);

        var property = ((Result<Property>.Success)createResult).Value;

        foreach (var url in request.ImageUrls ?? new List<string>())
            property.AddImage(url);

        await _repository.AddAsync(property);

        return Result.Success(PropertyMapper.ToDto(property, title,
            request.Latitude, request.Longitude, request.Stratum, request.Services));
    }

    public async Task<Result<PropertyDto>> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return Result.Failure<PropertyDto>("ID de propiedad inválido");

        var property = await _repository.FindByIdAsync(PropertyId.From(guid));
        if (property == null)
            return Result.Failure<PropertyDto>("Propiedad no encontrada");

        property.IncrementViews();
        await _repository.UpdateAsync(property);

        return Result.Success(PropertyMapper.ToDto(property));
    }

    public async Task<IEnumerable<PropertyDto>> SearchAsync(PropertyFilterRequest filter)
    {
        PropertyType? propType = null;
        if (!string.IsNullOrEmpty(filter.Type) && Enum.TryParse<PropertyType>(filter.Type, out var pt))
            propType = pt;

        TransactionType? txType = null;
        if (!string.IsNullOrEmpty(filter.Transaction) && Enum.TryParse<TransactionType>(filter.Transaction, out var tt))
            txType = tt;

        var searchFilter = new SearchFilter
        {
            City = filter.City ?? string.Empty,
            Neighborhood = filter.Neighborhood ?? string.Empty,
            Type = propType,
            Transaction = txType,
            MinPrice = filter.MinPrice,
            MaxPrice = filter.MaxPrice,
            OwnerId = filter.OwnerId
        };

        var results = await _repository.FindByFiltersAsync(searchFilter);
        // Solo propiedades verificadas son visibles públicamente, a menos que el filtro sea por un propietario específico (Dashboard)
        return results
            .Where(p => p.Status == VerificationStatus.Verified || (!string.IsNullOrEmpty(filter.OwnerId) && p.OwnerId.Value.ToString() == filter.OwnerId))
            .OrderByDescending(p => p.IsLocalPriority)
            .ThenByDescending(p => p.Views)
            .Select(p => PropertyMapper.ToDto(p));
    }

    public async Task<Result<PropertyDto>> UpdateAsync(string id, UpdatePropertyRequest request, string requestingUserId)
    {
        if (!Guid.TryParse(id, out var guid))
            return Result.Failure<PropertyDto>("ID de propiedad inválido");

        var property = await _repository.FindByIdAsync(PropertyId.From(guid));
        if (property == null)
            return Result.Failure<PropertyDto>("Propiedad no encontrada");

        // Solo el propietario puede editar (spec: properties.spec.md)
        if (property.OwnerId.Value.ToString() != requestingUserId)
            return Result.Failure<PropertyDto>("No tienes permiso para editar esta propiedad");

        // Aplicar imágenes si se enviaron
        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            foreach (var url in request.ImageUrls)
                property.AddImage(url);
        }

        await _repository.UpdateAsync(property);
        return Result.Success(PropertyMapper.ToDto(property));
    }

    public async Task<Result<bool>> DeleteAsync(string id, string requestingUserId, bool isAdmin)
    {
        if (!Guid.TryParse(id, out var guid))
            return Result.Failure<bool>("ID de propiedad inválido");

        var property = await _repository.FindByIdAsync(PropertyId.From(guid));
        if (property == null)
            return Result.Failure<bool>("Propiedad no encontrada");

        var isOwner = property.OwnerId.Value.ToString() == requestingUserId;
        if (!isOwner && !isAdmin)
            return Result.Failure<bool>("No tienes permiso para eliminar esta propiedad");

        await _repository.DeleteAsync(property);
        return Result.Success(true);
    }
}
