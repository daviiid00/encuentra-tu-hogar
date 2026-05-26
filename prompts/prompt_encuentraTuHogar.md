# PROMPT: Plataforma Web "Encuentra Tu Hogar" en C# con Programación Funcional-Orientada a Objetos

## 1. CONTEXTO DEL PROYECTO

Desarrollar una **plataforma web de arriendos directos y seguros** llamada "Encuentra Tu Hogar" que conecta arrendadores y arrendatarios de forma directa, priorizando habitantes locales, seguridad digital y confianza humana.

**Tecnología Stack:**
- Backend: ASP.NET Core 8.0 (C#)
- Frontend: Razor Pages + JavaScript/TypeScript
- Base de datos: SQL Server o PostgreSQL
- Arquitectura: Funcional-Orientada a Objetos con Domain-Driven Design (DDD)

---

## 2. DISEÑO VISUAL Y ESTRUCTURA

### 2.1 Componentes Principales de la UI
Basado en el prototipo proporcionado, la página debe incluir:

#### Header/Navegación
```
- Logo: "Encuentra Tu Hogar"
- Menú: [Inicio] [Cómo Funciona] [Experiencias] [Ayuda] [Ingresar]
- Botón CTA: "Ingresar" (Azul marino #1B3B5C)
```

#### Hero Section
```
- Título: "Encuentra tu vivienda rápida y fácil"
- Subtítulo: "Soluciones a tu alcance"
- Buscador interactivo con filtros:
  * Dropdown: Ciudad
  * Dropdown: Tipo de vivienda
  * Range Slider: Rango de precio
  * Botón: "Buscar" (Verde #4A9D6F)
```

#### Sección Propietarios
```
- Título: "¿Eres Propietario?"
- Subtítulo: "Quiero Ofertar mi Vivienda"
- Lista de beneficios:
  • Publica gratis tu propiedad en minutos
  • Llega a cientos de inquilinos
  • Gestiona tus interesados sin complicaciones
- Botón CTA: "Publicar mi propiedad"
```

#### Propiedades Destacadas (4 cards)
```
1. [Busca y Explora] - Encuentra opciones | Ícono: lupa
2. [Registrate Fácil] - Guarda tus datos | Ícono: formulario
3. [Agenda una Visita] - Programa tu cita | Ícono: calendario
4. [Conéctate con la Comunidad] - Recibe apoyo | Ícono: personas
```

#### Experiencias de Usuarios
```
- Slider/Carrusel con testimonios
- Nombre, foto, citación
- Mínimo 2 testimonios visibles
```

#### Chat Bot
- Ícono flotante en esquina inferior derecha
- Placeholder: "¿Cómo podemos ayudarte?"

---

## 3. ARQUITECTURA FUNCIONAL-ORIENTADA A OBJETOS

### 3.1 Estructura de Carpetas
```
EncuentraTuHogar/
├── src/
│   ├── Domain/                          # Entidades y Lógica de Negocio
│   │   ├── Entities/
│   │   │   ├── Property.cs             # Propiedad/Vivienda
│   │   │   ├── User.cs                 # Usuario (Arrendador/Arrendatario)
│   │   │   ├── Listing.cs              # Anuncio de propiedad
│   │   │   ├── Visit.cs                # Visita programada
│   │   │   └── Review.cs               # Reseña/Testimonio
│   │   ├── ValueObjects/
│   │   │   ├── Address.cs              # Dirección (Ciudad, zona)
│   │   │   ├── Price.cs                # Rango de precio
│   │   │   ├── PropertyType.cs         # Enum: Apartamento, Casa, etc.
│   │   │   └── VerificationStatus.cs   # Verificado, Pendiente, etc.
│   │   └── Services/
│   │       ├── IPropertyValidator.cs   # Validación de propiedades
│   │       └── IPricingCalculator.cs   # Cálculo de comisión
│   │
│   ├── Application/                     # Casos de Uso y Lógica Aplicativa
│   │   ├── DTOs/
│   │   │   ├── CreateListingDto.cs
│   │   │   ├── SearchPropertyDto.cs
│   │   │   ├── ScheduleVisitDto.cs
│   │   │   └── SubmitReviewDto.cs
│   │   ├── UseCases/
│   │   │   ├── CreateListingUseCase.cs
│   │   │   ├── SearchPropertiesUseCase.cs
│   │   │   ├── ScheduleVisitUseCase.cs
│   │   │   └── SubmitReviewUseCase.cs
│   │   └── Interfaces/
│   │       ├── IPropertyRepository.cs
│   │       ├── IUserRepository.cs
│   │       └── IVisitRepository.cs
│   │
│   ├── Infrastructure/                  # Implementación de Repositories y Servicios
│   │   ├── Persistence/
│   │   │   ├── PropertyRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   ├── AppDbContext.cs
│   │   │   └── DatabaseMigrations/
│   │   ├── Services/
│   │   │   ├── EmailService.cs         # Verificación y notificaciones
│   │   │   ├── SecurityService.cs      # Encriptación de datos
│   │   │   ├── ChatBotService.cs       # Respuestas automáticas
│   │   │   └── CloudStorageService.cs  # Fotos de propiedades
│   │   └── Mappers/
│   │       └── PropertyMapper.cs        # Mapeo Entity → DTO
│   │
│   └── Presentation/                    # Capa de Presentación (Web)
│       ├── Pages/
│       │   ├── Index.cshtml             # Página principal
│       │   ├── BuscarPropiedad.cshtml   # Búsqueda y filtros
│       │   ├── CrearAnuncio.cshtml      # Publicar propiedad
│       │   ├── DetallePropiedad.cshtml  # Detalle y galería
│       │   ├── AgendarVisita.cshtml     # Agendar visita
│       │   └── MisExperiencias.cshtml   # Testimonios
│       ├── Controllers/
│       │   ├── PropertyController.cs
│       │   ├── UserController.cs
│       │   ├── VisitController.cs
│       │   └── ChatController.cs
│       ├── ViewModels/
│       │   ├── PropertyViewModel.cs
│       │   └── SearchFilterViewModel.cs
│       ├── wwwroot/
│       │   ├── css/
│       │   │   ├── styles.css           # Colores: #1B3B5C, #4A9D6F, #F4A5A5
│       │   │   └── responsive.css       # Diseño responsivo
│       │   ├── js/
│       │   │   ├── search-filter.js     # Lógica de búsqueda
│       │   │   ├── chatbot.js           # Chat interactivo
│       │   │   └── gallery.js           # Galería de imágenes
│       │   └── images/
│       │       ├── logo.svg
│       │       └── icons/
│       └── Startup.cs / Program.cs
```

---

## 4. PRINCIPIOS DE PROGRAMACIÓN FUNCIONAL-ORIENTADA A OBJETOS

### 4.1 Patrones a Implementar

#### A. Domain-Driven Design (DDD)
```csharp
// Entidad de Dominio
public class Property : Entity
{
    public PropertyId Id { get; private set; }
    public Address Address { get; private set; }
    public PropertyType Type { get; private set; }
    public Price Price { get; private set; }
    public UserId OwnerId { get; private set; }
    public VerificationStatus Status { get; private set; }
    
    // Métodos del Dominio (Funcionales)
    public static Result<Property> Create(
        PropertyId id,
        Address address,
        PropertyType type,
        Price price,
        UserId ownerId)
    {
        return new Property { 
            Id = id, 
            Address = address, 
            Type = type, 
            Price = price, 
            OwnerId = ownerId,
            Status = VerificationStatus.Pending 
        };
    }
}
```

#### B. Use Cases (CQRS Simplificado)
```csharp
// Interface de Use Case
public interface IUseCase<in TRequest, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request);
}

// Implementación funcional
public class SearchPropertiesUseCase : IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>>
{
    private readonly IPropertyRepository _repository;
    
    public async Task<Result<IEnumerable<PropertyDto>>> ExecuteAsync(SearchPropertiesRequest request)
    {
        return await _repository
            .FindByFiltersAsync(
                request.City,
                request.PropertyType,
                request.PriceRange)
            .Map(properties => properties.Select(PropertyMapper.ToDto).ToList());
    }
}
```

#### C. Result Pattern (para manejo de errores funcional)
```csharp
public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string Error) : Result<T>;
}

// Uso
public Result<PropertyDto> GetProperty(int id)
{
    return _repository.FindById(id) switch
    {
        null => new Result<PropertyDto>.Failure("Propiedad no encontrada"),
        var property => new Result<PropertyDto>.Success(PropertyMapper.ToDto(property))
    };
}
```

#### D. Funciones de Orden Superior
```csharp
// Pipeline de validación funcional
public static class ValidationPipeline
{
    public static Func<T, Result<T>> CreatePipeline<T>(
        params Func<T, Result<T>>[] validators)
    {
        return item => validators.Aggregate(
            Result.Success(item),
            (acc, validator) => acc.FlatMap(validator));
    }
}
```

#### E. Repository Pattern con Funciones
```csharp
public interface IPropertyRepository
{
    Task<IEnumerable<Property>> FindByFiltersAsync(
        string city,
        PropertyType? type,
        PriceRange? range);
    
    Task<Property> FindByIdAsync(PropertyId id);
    
    Task AddAsync(Property property);
}
```

---

## 5. FUNCIONALIDADES PRINCIPALES A IMPLEMENTAR

### 5.1 Búsqueda de Propiedades
- **Filtros:** Ciudad, Tipo de vivienda, Rango de precio
- **Resultados:** Grid de propiedades con imagen, precio, ubicación
- **Verificación:** Mostrar estado de verificación
- **Prioridad Local:** Destacar propiedades de habitantes locales

### 5.2 Crear Anuncio (Propietarios)
- **Formulario:** Datos básicos, fotos, descripción, precio, comisión esperada (3-5%)
- **Validaciones:** 
  - Email verificado
  - Identidad verificada
  - Mínimo 3 fotos
- **Almacenamiento:** Subir imágenes a cloud (Azure Blob Storage)

### 5.3 Agendar Visita
- **Calendario:** Disponibilidad del propietario
- **Confirmación:** Email a ambas partes
- **Recordatorio:** 24 horas antes
- **Chat seguro:** Integración para comunicación previa

### 5.4 Sistema de Reseñas
- **Después de la visita:** Permitir calificación 1-5 estrellas
- **Comentarios:** Máximo 500 caracteres
- **Verificación:** Solo usuarios que visitaron pueden reseñar
- **Mostrar en Home:** Carrusel con testimonios

### 5.5 Chat Seguro Integrado
- **Cifrado end-to-end:** Para mensajes entre usuario y propietario
- **Chat Bot:** Respuestas automáticas para preguntas frecuentes
- **Notificaciones:** Real-time
- **Historial:** Almacenar conversaciones (con consentimiento)

### 5.6 Seguridad y Verificación
- **Verificación de Email:** Link de confirmación
- **Verificación de Identidad:** Por documento o teléfono
- **Encriptación:** Datos sensibles en base de datos
- **GDPR Compliance:** Política de privacidad clara

---

## 6. ESPECIFICACIONES TÉCNICAS

### 6.1 Base de Datos
```sql
-- Tablas principales
Users (UserId, Email, PasswordHash, VerificationStatus, CreatedAt)
Properties (PropertyId, UserId, Address, Type, Price, Status, CreatedAt)
Listings (ListingId, PropertyId, Description, MainImage, Status)
Visits (VisitId, PropertyId, VisitorId, ScheduledDate, Status)
Reviews (ReviewId, VisitId, Rating, Comment, CreatedAt)
Messages (MessageId, SenderId, ReceiverId, Content, CreatedAt, IsEncrypted)
```

### 6.2 APIs REST
```
GET  /api/properties/search?city=&type=&minPrice=&maxPrice=
GET  /api/properties/{id}
POST /api/properties (Crear anuncio)
GET  /api/visits/schedule?propertyId={id}
POST /api/visits (Agendar visita)
POST /api/reviews
POST /api/messages/send
GET  /api/messages/{conversationId}
```

### 6.3 Estilos y Paleta de Colores
```
Color Primario:     #1B3B5C (Azul marino - Confianza, seguridad)
Color Secundario:   #4A9D6F (Verde - Acción, crecimiento)
Color Terciario:    #F4A5A5 (Rosa suave - Amabilidad, comunidad)
Fondo Claro:        #F8F9FA
Texto Principal:    #2C3E50
Texto Secundario:   #7F8C8D
Bordes:             #E0E0E0
Éxito:              #27AE60
Error:              #E74C3C
```

### 6.4 Tipografía
```
Titulares:  Segoe UI, sans-serif, Bold 28-32px
Subtítulos: Segoe UI, sans-serif, Medium 18-20px
Body Text:  Segoe UI, sans-serif, Regular 14-16px
```

---

## 7. METODOLOGÍA DE DESARROLLO

### 7.1 Orden de Implementación
1. **Fase 1:** Estructura base, DDD entities, repositories
2. **Fase 2:** Use cases y lógica de negocio
3. **Fase 3:** Controllers y Razor Pages
4. **Fase 4:** Integración de servicios (Email, Storage, Chat)
5. **Fase 5:** Tests unitarios y de integración
6. **Fase 6:** Seguridad y verificación
7. **Fase 7:** Optimización y deployment

### 7.2 Patrones a Seguir
- **Single Responsibility Principle:** Cada clase tiene una responsabilidad
- **Dependency Injection:** Constructor injection
- **Immutability:** Records y propiedades readonly donde sea posible
- **Composición sobre Herencia:** Usar interfaces y combinación
- **Testing First:** Escribir tests junto con el código

---

## 8. REQUISITOS NO FUNCIONALES

- **Performance:** Búsqueda en <500ms
- **Disponibilidad:** 99.5% uptime
- **Seguridad:** HTTPS, SQL Injection prevention, XSS protection
- **Escalabilidad:** Arquitectura preparada para microservicios
- **Responsividad:** Mobile-first (Bootstrap 5 o Tailwind)
- **Accesibilidad:** WCAG 2.1 AA compliance

---

## 9. EJEMPLO DE CÓDIGO FUNCIONAL IMPLEMENTADO

```csharp
// Domain Service: Validación de Propiedad
public static class PropertyValidation
{
    public static Result<Property> ValidateProperty(Property property)
    {
        return ValidateAddress(property.Address)
            .FlatMap(_ => ValidatePrice(property.Price))
            .FlatMap(_ => ValidatePropertyType(property.Type))
            .Map(_ => property);
    }
    
    private static Result<Address> ValidateAddress(Address address) =>
        string.IsNullOrEmpty(address.City) || string.IsNullOrEmpty(address.Zone)
            ? Result.Failure<Address>("Dirección inválida")
            : Result.Success(address);
    
    private static Result<Price> ValidatePrice(Price price) =>
        price.Amount <= 0
            ? Result.Failure<Price>("Precio debe ser positivo")
            : Result.Success(price);
    
    private static Result<PropertyType> ValidatePropertyType(PropertyType type) =>
        Enum.IsDefined(type)
            ? Result.Success(type)
            : Result.Failure<PropertyType>("Tipo de propiedad inválido");
}

// Use Case: Crear Anuncio
public class CreateListingUseCase : IUseCase<CreateListingRequest, CreateListingResponse>
{
    private readonly IPropertyRepository _repository;
    private readonly IPropertyValidator _validator;
    
    public async Task<Result<CreateListingResponse>> ExecuteAsync(CreateListingRequest request)
    {
        return await CreatePropertyFromRequest(request)
            .FlatMap(_validator.ValidateAsync)
            .FlatMapAsync(async property => 
            {
                await _repository.AddAsync(property);
                return Result.Success(MapToResponse(property));
            });
    }
    
    private Result<Property> CreatePropertyFromRequest(CreateListingRequest request) =>
        Property.Create(
            PropertyId.New(),
            new Address(request.City, request.Zone),
            request.Type,
            new Price(request.Amount),
            request.OwnerId);
}
```

---

## 10. ENTREGABLES ESPERADOS

✅ Solución C# ASP.NET Core con estructura DDD completa
✅ Página web funcional con diseño responsivo (mobile, tablet, desktop)
✅ Base de datos con tablas y relaciones
✅ Implementación de funcionalidades principales (búsqueda, anuncio, visitas, reseñas)
✅ Sistema de chat seguro con chat bot básico
✅ Tests unitarios de casos de uso críticos
✅ Documentación de API (Swagger)
✅ Script de deployment en Azure/AWS

---

## NOTAS IMPORTANTES

- Mantener la **esencia funcional**: Usar records, pattern matching, expresiones lambda
- Evitar mutations innecesarias
- Priorizar **composición funcional** en validaciones
- Implementar **error handling explícito** con Result pattern
- Asegurar **separación clara** entre capas
- Documentar **cada use case** con ejemplos de entrada/salida
