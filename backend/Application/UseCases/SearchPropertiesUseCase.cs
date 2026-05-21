using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Application.Mappers;
using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Application.UseCases;

public class SearchPropertiesUseCase : IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    
    public SearchPropertiesUseCase(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }
    
    public async Task<Result<IEnumerable<PropertyDto>>> ExecuteAsync(SearchPropertiesRequest request)
    {
        return await ValidateRequest(request)
            .FlatMapAsync(async _ => await SearchAndMapResults(request));
    }
    
    private Result<Unit> ValidateRequest(SearchPropertiesRequest request) =>
        string.IsNullOrEmpty(request.City)
            ? Result.Failure<Unit>("Ciudad es requerida")
            : Result.Success(Unit.Value);
            
    private async Task<Result<IEnumerable<PropertyDto>>> SearchAndMapResults(SearchPropertiesRequest request)
    {
        try
        {
            var filter = new SearchFilter
            {
                City = request.City,
                Type = request.Type,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice
            };
            
            var properties = await _propertyRepository.FindByFiltersAsync(filter);
            
            var results = properties
                .Where(p => p.Status == VerificationStatus.Verified)
                .OrderByDescending(p => p.IsLocalPriority)
                .ThenByDescending(p => p.Views)
                .Select(PropertyMapper.ToDto)
                .ToList();
                
            return Result.Success<IEnumerable<PropertyDto>>(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<PropertyDto>>("Error al buscar propiedades: " + ex.Message);
        }
    }
}
