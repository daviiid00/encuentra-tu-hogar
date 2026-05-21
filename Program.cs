using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Application.UseCases;
using EncuentraTuHogar.API.Middleware;
using EncuentraTuHogar.Infrastructure.Extensions;
using EncuentraTuHogar.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ── Razor Pages (frontend existente — sin modificar) ──────────────────────────
builder.Services.AddRazorPages();

// ── Controladores REST API ────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Base de datos (SQLite — preparado para migrar a PostgreSQL) ───────────────
builder.Services.AddDatabase(builder.Configuration);

// ── Identity + Roles ──────────────────────────────────────────────────────────
builder.Services.AddIdentityServices();

// ── Authentication (JWT + Cookies) ────────────────────────────────────────────
builder.Services.AddHybridAuthentication(builder.Configuration);

// ── Frontend API Client ───────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<EncuentraTuHogar.Frontend.Services.ApiClient>(client =>
{
    // Using current localhost standard ports. Adjust if deployed.
    client.BaseAddress = new Uri("http://localhost:5035");
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCorsPolicy(builder.Configuration);

// ── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddRepositories();

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddApplicationServices();

// ── Use Cases (legacy — se mantienen para compatibilidad) ─────────────────────
builder.Services.AddScoped<
    IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>>,
    SearchPropertiesUseCase>();

// ── SignalR (hub de chat — sin WebSockets todavía) ────────────────────────────
builder.Services.AddSignalR();

// ── API Explorer (preparado para Swagger futuro) ──────────────────────────────
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────

// Middleware global de errores (antes de todo)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");

app.UseRouting();

// Autenticación ANTES de autorización (orden obligatorio)
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();              // Razor Pages (UI sin modificar)
app.MapControllers();                                 // REST API controllers
app.MapHub<CommunityChatHub>("/communityChatHub");   // SignalR (futuro)

app.Run();
