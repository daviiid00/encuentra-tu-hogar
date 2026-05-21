using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EncuentraTuHogar.Infrastructure.Persistence;
using EncuentraTuHogar.Infrastructure.Identity;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Application.UseCases;
using EncuentraTuHogar.Application.DTOs;
using System.Collections.Generic;
using EncuentraTuHogar.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=EncuentraTuHogar.db"));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();

// UseCases
builder.Services.AddScoped<IUseCase<SearchPropertiesRequest, IEnumerable<PropertyDto>>, SearchPropertiesUseCase>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapHub<CommunityChatHub>("/communityChatHub");

app.Run();
