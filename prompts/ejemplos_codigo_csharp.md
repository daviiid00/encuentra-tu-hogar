# EJEMPLOS DE CÓDIGO - Plataforma "Encuentra Tu Hogar"

## 1. ENTIDADES DE DOMINIO CON FUNCIONALIDAD

### 1.1 Value Objects (Inmutables)

```csharp
// Domain/ValueObjects/Address.cs
public record Address
{
    public string City { get; init; }
    public string Zone { get; init; }
    public string Street { get; init; }
    public string PostalCode { get; init; }
    
    public Address(string city, string zone, string street, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(city)) 
            throw new ArgumentException("Ciudad requerida");
        if (string.IsNullOrWhiteSpace(zone)) 
            throw new ArgumentException("Zona requerida");
            
        City = city;
        Zone = zone;
        Street = street;
        PostalCode = postalCode;
    }
    
    // Función pura para validación
    public bool IsValid() => 
        !string.IsNullOrEmpty(City) && 
        !string.IsNullOrEmpty(Zone) && 
        !string.IsNullOrEmpty(Street);
}

// Domain/ValueObjects/Price.cs
public record Price
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    
    public Price(decimal amount, string currency = "COP")
    {
        if (amount <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");
            
        Amount = amount;
        Currency = currency;
    }
    
    // Función pura para calcular comisión
    public Price CalculateCommission(decimal percentage) =>
        new Price(Amount * (percentage / 100), Currency);
    
    public override string ToString() => 
        $"{Currency} ${Amount:N0}";
}

// Domain/ValueObjects/PropertyType.cs
public enum PropertyType
{
    Apartment = 1,
    House = 2,
    Studio = 3,
    Room = 4,
    Loft = 5,
    Townhouse = 6
}

// Domain/ValueObjects/VerificationStatus.cs
public enum VerificationStatus
{
    Pending = 0,
    Verified = 1,
    Rejected = 2,
    Suspended = 3
}
```

### 1.2 Entidad Principal (Property)

```csharp
// Domain/Entities/Property.cs
public class Property : Entity
{
    public PropertyId Id { get; private set; }
    public Address Address { get; private set; }
    public PropertyType Type { get; private set; }
    public Price Price { get; private set; }
    public UserId OwnerId { get; private set; }
    public VerificationStatus Status { get; private set; }
    public string Description { get; private set; }
    public List<string> ImageUrls { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public int Views { get; private set; }
    public bool IsLocalPriority { get; private set; }
    
    // Constructor privado para DDD
    private Property() { }
    
    // Factory Method funcional
    public static Result<Property> Create(
        Address address,
        PropertyType type,
        Price price,
        UserId ownerId,
        string description)
    {
        return ValidateInputs(address, type, price, ownerId, description)
            .Map(_ => new Property
            {
                Id = PropertyId.New(),
                Address = address,
                Type = type,
                Price = price,
                OwnerId = ownerId,
                Status = VerificationStatus.Pending,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                Views = 0,
                IsLocalPriority = false
            });
    }
    
    // Validación funcional
    private static Result<Unit> ValidateInputs(
        Address address,
        PropertyType type,
        Price price,
        UserId ownerId,
        string description)
    {
        return ValidateAddress(address)
            .FlatMap(_ => ValidatePrice(price))
            .FlatMap(_ => ValidateDescription(description))
            .Map(_ => Unit.Value);
    }
    
    private static Result<Unit> ValidateAddress(Address address) =>
        address.IsValid()
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Dirección inválida");
    
    private static Result<Unit> ValidatePrice(Price price) =>
        price.Amount > 0
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Precio debe ser positivo");
    
    private static Result<Unit> ValidateDescription(string description) =>
        !string.IsNullOrEmpty(description) && description.Length >= 20
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Descripción debe tener mínimo 20 caracteres");
    
    // Métodos de comportamiento
    public void AddImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            throw new ArgumentException("URL de imagen inválida");
        if (ImageUrls.Count >= 10)
            throw new InvalidOperationException("Máximo 10 imágenes permitidas");
            
        ImageUrls.Add(imageUrl);
    }
    
    public void IncrementViews() => Views++;
    
    public void MarkAsVerified() => Status = VerificationStatus.Verified;
    
    public void Reject(string reason) => Status = VerificationStatus.Rejected;
    
    public Price GetFinalPrice() => Price;
    
    // Predicado para filtrado funcional
    public bool MatchesFilter(SearchFilter filter) =>
        filter.City.ToLower() == Address.City.ToLower() &&
        (filter.Type == null || filter.Type == Type) &&
        (filter.MinPrice == null || Price.Amount >= filter.MinPrice) &&
        (filter.MaxPrice == null || Price.Amount <= filter.MaxPrice);
}

// Value Object para ID (Strongly Typed ID)
public record PropertyId
{
    public Guid Value { get; }
    
    private PropertyId(Guid value) => Value = value;
    
    public static PropertyId New() => new(Guid.NewGuid());
    
    public static PropertyId From(Guid id) => new(id);
    
    public override string ToString() => Value.ToString();
}
```

---

## 2. USE CASES (CASOS DE USO)

### 2.1 Búsqueda de Propiedades

```csharp
// Application/UseCases/SearchPropertiesUseCase.cs
public class SearchPropertiesUseCase : IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<SearchPropertiesUseCase> _logger;
    
    public SearchPropertiesUseCase(
        IPropertyRepository propertyRepository,
        ILogger<SearchPropertiesUseCase> logger)
    {
        _propertyRepository = propertyRepository;
        _logger = logger;
    }
    
    public async Task<Result<IEnumerable<PropertyDto>>> ExecuteAsync(
        SearchPropertiesRequest request)
    {
        return await ValidateRequest(request)
            .FlatMapAsync(async _ => 
                await SearchAndMapResults(request))
            .TapAsync(results => LogSearchMetrics(request, results));
    }
    
    private Result<Unit> ValidateRequest(SearchPropertiesRequest request) =>
        string.IsNullOrEmpty(request.City)
            ? Result.Failure<Unit>("Ciudad es requerida")
            : Result.Success(Unit.Value);
    
    private async Task<Result<IEnumerable<PropertyDto>>> SearchAndMapResults(
        SearchPropertiesRequest request)
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
            
            var properties = await _propertyRepository
                .FindByFiltersAsync(filter);
            
            var results = properties
                .Where(p => p.Status == VerificationStatus.Verified)
                .OrderByPriority()           // Prioridad local primero
                .OrderByViews()              // Más relevancia
                .Select(PropertyMapper.ToDto)
                .ToList();
            
            return Result.Success<IEnumerable<PropertyDto>>(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda de propiedades");
            return Result.Failure<IEnumerable<PropertyDto>>(
                "Error al buscar propiedades");
        }
    }
    
    private async Task LogSearchMetrics(
        SearchPropertiesRequest request,
        IEnumerable<PropertyDto> results)
    {
        _logger.LogInformation(
            "Búsqueda: Ciudad={City}, Resultados={Count}",
            request.City,
            results.Count());
    }
}

// Application/DTOs/SearchPropertiesRequest.cs
public class SearchPropertiesRequest
{
    public string City { get; set; }
    public PropertyType? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Application/DTOs/PropertyDto.cs
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
```

### 2.2 Crear Anuncio

```csharp
// Application/UseCases/CreateListingUseCase.cs
public class CreateListingUseCase : IUseCase<CreateListingRequest, CreateListingResponse>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICloudStorageService _storageService;
    private readonly IUserRepository _userRepository;
    
    public async Task<Result<CreateListingResponse>> ExecuteAsync(
        CreateListingRequest request)
    {
        return await ValidateOwnerExists(request.OwnerId)
            .FlatMap(_ => CreatePropertyFromRequest(request))
            .FlatMapAsync(async property => 
                await ProcessAndSaveProperty(property, request))
            .TapAsync(async response => 
                await SendConfirmationEmail(response));
    }
    
    private async Task<Result<Unit>> ValidateOwnerExists(UserId ownerId)
    {
        var owner = await _userRepository.FindByIdAsync(ownerId);
        return owner == null
            ? Result.Failure<Unit>("Usuario propietario no encontrado")
            : owner.VerificationStatus != VerificationStatus.Verified
                ? Result.Failure<Unit>("Usuario debe estar verificado")
                : Result.Success(Unit.Value);
    }
    
    private Result<Property> CreatePropertyFromRequest(CreateListingRequest request) =>
        Property.Create(
            new Address(request.City, request.Zone, request.Street, request.PostalCode),
            request.Type,
            new Price(request.Amount),
            request.OwnerId,
            request.Description);
    
    private async Task<Result<CreateListingResponse>> ProcessAndSaveProperty(
        Property property,
        CreateListingRequest request)
    {
        // Procesar imágenes
        var uploadResults = await Task.WhenAll(
            request.ImageFiles.Select(img => 
                _storageService.UploadImageAsync(img))
        );
        
        var imageUrls = uploadResults
            .Where(r => r.IsSuccess)
            .Select(r => r.Url)
            .ToList();
        
        if (imageUrls.Count == 0)
            return Result.Failure<CreateListingResponse>(
                "Debe subir al menos una imagen");
        
        // Agregar imágenes a la propiedad
        foreach (var url in imageUrls)
            property.AddImage(url);
        
        // Guardar en BD
        await _propertyRepository.AddAsync(property);
        
        return Result.Success(new CreateListingResponse(
            property.Id.Value,
            property.Address.City,
            property.Price.ToString(),
            "Anuncio creado exitosamente"));
    }
    
    private async Task SendConfirmationEmail(CreateListingResponse response)
    {
        // Lógica de envío de email
    }
}

public record CreateListingRequest(
    string City,
    string Zone,
    string Street,
    string PostalCode,
    PropertyType Type,
    decimal Amount,
    string Description,
    UserId OwnerId,
    IFormFile[] ImageFiles);

public record CreateListingResponse(
    Guid PropertyId,
    string City,
    string Price,
    string Message);
```

### 2.3 Agendar Visita

```csharp
// Application/UseCases/ScheduleVisitUseCase.cs
public class ScheduleVisitUseCase : IUseCase<ScheduleVisitRequest, ScheduleVisitResponse>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IEmailService _emailService;
    
    public async Task<Result<ScheduleVisitResponse>> ExecuteAsync(
        ScheduleVisitRequest request)
    {
        return await ValidatePropertyAndDate(request)
            .FlatMap(_ => CheckPropertyAvailability(request))
            .FlatMapAsync(async property => 
                await CreateAndSaveVisit(property, request))
            .TapAsync(async visit => 
                await NotifyOwnerAndVisitor(visit));
    }
    
    private async Task<Result<Property>> ValidatePropertyAndDate(
        ScheduleVisitRequest request)
    {
        var property = await _propertyRepository.FindByIdAsync(
            PropertyId.From(request.PropertyId));
        
        if (property == null)
            return Result.Failure<Property>("Propiedad no encontrada");
        
        if (request.ScheduledDate < DateTime.UtcNow.AddHours(24))
            return Result.Failure<Property>(
                "Debe agendar mínimo 24 horas antes");
        
        return Result.Success(property);
    }
    
    private async Task<Result<Property>> CheckPropertyAvailability(
        ScheduleVisitRequest request)
    {
        var existingVisits = await _visitRepository
            .FindByPropertyAndDateAsync(
                PropertyId.From(request.PropertyId),
                request.ScheduledDate);
        
        const int maxVisitsPerDay = 5;
        
        return existingVisits.Count >= maxVisitsPerDay
            ? Result.Failure<Property>("No hay disponibilidad para esa fecha")
            : Result.Success(await _propertyRepository.FindByIdAsync(
                PropertyId.From(request.PropertyId)));
    }
    
    private async Task<Result<Visit>> CreateAndSaveVisit(
        Property property,
        ScheduleVisitRequest request)
    {
        var visit = Visit.Create(
            PropertyId.From(request.PropertyId),
            UserId.From(request.VisitorId),
            request.ScheduledDate);
        
        await _visitRepository.AddAsync(visit);
        return Result.Success(visit);
    }
    
    private async Task NotifyOwnerAndVisitor(Visit visit)
    {
        // Enviar emails de confirmación a ambas partes
        // Programar recordatorio para 24 horas antes
    }
}
```

---

## 3. SERVICIOS DE APLICACIÓN

### 3.1 Validación Funcional

```csharp
// Domain/Services/PropertyValidator.cs
public interface IPropertyValidator
{
    Task<Result<Unit>> ValidateAsync(Property property);
}

public class PropertyValidator : IPropertyValidator
{
    private readonly IPropertyRepository _repository;
    
    public PropertyValidator(IPropertyRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<Unit>> ValidateAsync(Property property) =>
        await ChainValidations(property);
    
    private async Task<Result<Unit>> ChainValidations(Property property)
    {
        return ValidateAddressFormat(property.Address)
            .FlatMap(_ => ValidatePriceRange(property.Price))
            .FlatMap(_ => ValidateDescription(property.Description))
            .FlatMapAsync(_ => ValidateOwnerRatings(property.OwnerId))
            .FlatMapAsync(_ => ValidateDuplicates(property));
    }
    
    private Result<Unit> ValidateAddressFormat(Address address)
    {
        var isValid = !string.IsNullOrEmpty(address.City) &&
                      !string.IsNullOrEmpty(address.Zone) &&
                      address.City.Length >= 3;
        
        return isValid
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Dirección inválida");
    }
    
    private Result<Unit> ValidatePriceRange(Price price)
    {
        const decimal minPrice = 100000;  // COP
        const decimal maxPrice = 50000000; // COP
        
        return price.Amount >= minPrice && price.Amount <= maxPrice
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>($"Precio debe estar entre {minPrice} y {maxPrice}");
    }
    
    private Result<Unit> ValidateDescription(string description)
    {
        const int minLength = 20;
        const int maxLength = 5000;
        
        return description.Length >= minLength && description.Length <= maxLength
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>($"Descripción debe tener entre {minLength} y {maxLength} caracteres");
    }
    
    private async Task<Result<Unit>> ValidateOwnerRatings(UserId ownerId)
    {
        var owner = await _repository.GetOwnerStatsAsync(ownerId);
        
        return owner.AverageRating >= 3.5
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>("Propietario debe tener calificación mínima de 3.5");
    }
    
    private async Task<Result<Unit>> ValidateDuplicates(Property property)
    {
        var duplicates = await _repository
            .FindSimilarPropertiesAsync(property.Address, property.Type);
        
        return duplicates.Any()
            ? Result.Failure<Unit>("Ya existe una propiedad similar en esta zona")
            : Result.Success(Unit.Value);
    }
}
```

### 3.2 Chat Service (Funcional)

```csharp
// Application/Services/ChatBotService.cs
public interface IChatBotService
{
    Task<string> GetResponseAsync(string message, UserId userId);
}

public class ChatBotService : IChatBotService
{
    private readonly Dictionary<string, Func<string>> _responses;
    
    public ChatBotService()
    {
        _responses = new()
        {
            { "precio", () => "¿Cuál es tu presupuesto? Podemos ayudarte a encontrar propiedades dentro de tu rango." },
            { "zona", () => "¿En qué zona de Medellín buscas? Tenemos opciones en todas las comunas." },
            { "documentos", () => "Necesitamos: Cédula, comprobante de ingresos y referencias." },
            { "comisión", () => "La comisión es entre 3% y 5%, según el tipo de servicio." },
            { "seguridad", () => "Verificamos identidad de todos los usuarios y encriptamos datos personales." }
        };
    }
    
    public async Task<string> GetResponseAsync(string message, UserId userId)
    {
        return await FindResponse(message)
            .Match(
                response => response(),
                () => GenerateDefaultResponse(message));
    }
    
    private Option<Func<string>> FindResponse(string message)
    {
        var lowerMessage = message.ToLower();
        
        return _responses
            .Keys
            .Where(key => lowerMessage.Contains(key))
            .FirstOrDefault() is { } matchedKey
                ? Option.Some(_responses[matchedKey])
                : Option.None<Func<string>>();
    }
    
    private string GenerateDefaultResponse(string message) =>
        message.Length < 5
            ? "Por favor, escribe una pregunta más detallada."
            : "No entiendo muy bien. ¿Podrías reformular tu pregunta? Puedo ayudarte con: precio, zona, documentos, comisión, seguridad.";
}
```

---

## 4. CONTROLLERS Y PRESENTACIÓN

### 4.1 Property Controller

```csharp
// Presentation/Controllers/PropertyController.cs
[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>> _searchUseCase;
    private readonly IUseCase<CreateListingRequest, CreateListingResponse> _createListingUseCase;
    private readonly ILogger<PropertyController> _logger;
    
    public PropertyController(
        IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>> searchUseCase,
        IUseCase<CreateListingRequest, CreateListingResponse> createListingUseCase,
        ILogger<PropertyController> logger)
    {
        _searchUseCase = searchUseCase;
        _createListingUseCase = createListingUseCase;
        _logger = logger;
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string city,
        [FromQuery] PropertyType? type,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var request = new SearchPropertiesRequest
        {
            City = city,
            Type = type,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };
        
        var result = await _searchUseCase.ExecuteAsync(request);
        
        return result switch
        {
            Result<IEnumerable<PropertyDto>>.Success success => 
                Ok(new { data = success.Value, count = success.Value.Count() }),
            
            Result<IEnumerable<PropertyDto>>.Failure failure => 
                BadRequest(new { error = failure.Error }),
            
            _ => StatusCode(500, new { error = "Error inesperado" })
        };
    }
    
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateListing(
        [FromForm] CreateListingRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request = request with { OwnerId = UserId.From(Guid.Parse(userId)) };
        
        var result = await _createListingUseCase.ExecuteAsync(request);
        
        return result switch
        {
            Result<CreateListingResponse>.Success success => 
                CreatedAtAction(nameof(GetById), 
                    new { id = success.Value.PropertyId }, 
                    success.Value),
            
            Result<CreateListingResponse>.Failure failure => 
                BadRequest(new { error = failure.Error }),
            
            _ => StatusCode(500, new { error = "Error al crear anuncio" })
        };
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Implementación
        return Ok();
    }
}
```

---

## 5. EXTENSIONES FUNCIONALES

### 5.1 Funciones de Extensión para Colecciones

```csharp
// Shared/Extensions/EnumerableExtensions.cs
public static class EnumerableExtensions
{
    // Ordenar por prioridad local
    public static IEnumerable<Property> OrderByPriority(
        this IEnumerable<Property> properties) =>
        properties.OrderByDescending(p => p.IsLocalPriority);
    
    // Ordenar por vistas (popularidad)
    public static IEnumerable<Property> OrderByViews(
        this IEnumerable<Property> properties) =>
        properties.OrderByDescending(p => p.Views);
    
    // Filtrar por rango de precio
    public static IEnumerable<Property> FilterByPrice(
        this IEnumerable<Property> properties,
        decimal? minPrice,
        decimal? maxPrice) =>
        properties.Where(p =>
            (minPrice == null || p.Price.Amount >= minPrice) &&
            (maxPrice == null || p.Price.Amount <= maxPrice));
    
    // Pipeline de transformación
    public static IEnumerable<T> Tap<T>(
        this IEnumerable<T> source,
        Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
            yield return item;
        }
    }
}

// Presentation/Extensions/PropertyExtensions.cs
public static class PropertyExtensions
{
    public static PropertyViewModel ToViewModel(this Property property) =>
        new PropertyViewModel
        {
            Id = property.Id.Value,
            City = property.Address.City,
            Zone = property.Address.Zone,
            Type = property.Type.ToString(),
            Price = property.Price.ToString(),
            Description = property.Description,
            MainImage = property.ImageUrls.FirstOrDefault(),
            Images = property.ImageUrls,
            Views = property.Views
        };
}
```

---

## 6. MAPPERS

```csharp
// Application/Mappers/PropertyMapper.cs
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
            0.0,  // AverageRating (se calcularía desde reviews)
            property.IsLocalPriority);
    
    public static IEnumerable<PropertyDto> ToDto(IEnumerable<Property> properties) =>
        properties.Select(ToDto);
}
```

Este archivo proporciona ejemplos concretos listos para implementar en el proyecto real.
