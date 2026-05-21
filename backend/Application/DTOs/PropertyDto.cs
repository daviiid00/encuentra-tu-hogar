namespace EncuentraTuHogar.Application.DTOs;

// Spec: properties.spec.md

public record PropertyDto(
    string Id,
    string Title,
    string Description,
    string City,
    string Neighborhood,
    string Street,
    decimal Price,
    string Currency,
    string Type,
    string Transaction,
    string Status,
    int Stratum,
    double? Latitude,
    double? Longitude,
    bool IsAvailable,
    bool IsLocalPriority,
    List<string> ImageUrls,
    List<string> Services,
    string OwnerId,
    int Views,
    DateTime CreatedAt
);

public record CreatePropertyRequest(
    string Title,
    string Description,
    string City,
    string Neighborhood,
    string Street,
    string PostalCode,
    decimal Price,
    string Type,          // "Apartment" | "House" | "Room" | "Studio"
    string Transaction,   // "Rent" | "Sale"
    int Stratum,
    double? Latitude,
    double? Longitude,
    List<string> Services,
    List<string> ImageUrls
);

public record UpdatePropertyRequest(
    string? Title,
    string? Description,
    decimal? Price,
    bool? IsAvailable,
    int? Stratum,
    List<string>? Services,
    List<string>? ImageUrls
);

public record PropertyFilterRequest(
    string? City,
    string? Type,
    string? Transaction,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? IsAvailable,
    string? OwnerId,
    int? Page,
    int? PageSize
);
