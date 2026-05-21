using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Application.DTOs;

public record PropertyDto(
    string Id,
    string City,
    string Zone,
    PropertyType Type,
    decimal Price,
    string MainImage,
    int Views,
    double AverageRating,
    bool IsLocalPriority);
