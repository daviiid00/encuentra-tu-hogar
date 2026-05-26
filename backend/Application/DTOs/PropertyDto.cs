using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 100 caracteres")]
    string Title,

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "La descripción debe tener al menos 20 caracteres")]
    string Description,

    [Required(ErrorMessage = "La ciudad es obligatoria")]
    string City,

    [Required(ErrorMessage = "El barrio es obligatorio")]
    string Neighborhood,

    [Required(ErrorMessage = "La dirección es obligatoria")]
    string Street,

    string PostalCode,

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(1000, 10000000000, ErrorMessage = "El precio debe ser un valor razonable")]
    decimal Price,

    [Required(ErrorMessage = "El tipo de propiedad es obligatorio")]
    string Type,          // "Apartment" | "House" | "Room" | "Studio"

    [Required(ErrorMessage = "El tipo de transacción es obligatorio")]
    string Transaction,   // "Rent" | "Sale"

    [Range(1, 6, ErrorMessage = "El estrato debe estar entre 1 y 6")]
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
    string? Neighborhood,
    string? Type,
    string? Transaction,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? IsAvailable,
    string? OwnerId,
    int? Page,
    int? PageSize
);
