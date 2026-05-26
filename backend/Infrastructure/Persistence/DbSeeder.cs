using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using EncuentraTuHogar.Infrastructure.Identity;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;

namespace EncuentraTuHogar.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // 1. Roles
            string[] roles = { "Landlord", "Tenant", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Users
            string ownerEmail = "propietario@demo.com";
            string tenantEmail = "inquilino@demo.com";
            
            var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
            if (ownerUser == null)
            {
                ownerUser = new ApplicationUser
                {
                    UserName = ownerEmail,
                    Email = ownerEmail,
                    FullName = "Carlos Propietario",
                    City = "Medellín",
                    IsOwner = true
                };
                await userManager.CreateAsync(ownerUser, "Demo123*");
                await userManager.AddToRoleAsync(ownerUser, "Landlord");
            }

            var tenantUser = await userManager.FindByEmailAsync(tenantEmail);
            if (tenantUser == null)
            {
                tenantUser = new ApplicationUser
                {
                    UserName = tenantEmail,
                    Email = tenantEmail,
                    FullName = "Ana Inquilina",
                    City = "Bogotá",
                    IsOwner = false
                };
                await userManager.CreateAsync(tenantUser, "Demo123*");
                await userManager.AddToRoleAsync(tenantUser, "Tenant");
            }

            // 3. Properties (solo Medellín)
            if (context.Properties.Count() < 9)
            {
                Property SafeExtract(Result<Property> result)
                {
                    if (result is Result<Property>.Success s) return s.Value;
                    var err = result is Result<Property>.Failure f ? f.Error : "desconocido";
                    throw new InvalidOperationException($"Error creando propiedad seed: {err}");
                }

                var p1 = SafeExtract(Property.Create(new Address("Medellín", "El Poblado", "Cra 43A #1-50", "050022"), PropertyType.Apartment, TransactionType.Rent, new Price(2500000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Amplio apartamento con excelente vista al parque, ideal para familias. Cuenta con zonas verdes y seguridad 24/7. Hermoso Apartamento en El Poblado"));
                var p2 = SafeExtract(Property.Create(new Address("Medellín", "Laureles", "Circular 4 #70-12", "050031"), PropertyType.House, TransactionType.Rent, new Price(3200000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Hermosa casa de dos niveles con patio interior, 3 habitaciones y 2 baños completos. Casa familiar en Laureles"));
                var p3 = SafeExtract(Property.Create(new Address("Medellín", "Laureles", "Av. El Poblado #32-15", "050031"), PropertyType.Studio, TransactionType.Rent, new Price(1800000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Aptoestudio completamente amoblado cerca de universidades, parques y comercio. Estudio moderno en Laureles"));
                var p4 = SafeExtract(Property.Create(new Address("Medellín", "Belén", "Cra 76 #30-20", "050016"), PropertyType.House, TransactionType.Rent, new Price(2200000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Casa amplia en barrio tranquilo, ideal para familia numerosa. 4 habitaciones, garaje y patio grande. Casa en Belén"));
                var p5 = SafeExtract(Property.Create(new Address("Medellín", "Estadio", "Cra 70 #45-08", "050034"), PropertyType.Apartment, TransactionType.Rent, new Price(1600000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Apartamento cómodo de 2 habitaciones, cerca al estadio Atanasio Girardot y universidades. Excelente transporte"));
                var p6 = SafeExtract(Property.Create(new Address("Medellín", "Robledo", "Calle 80 #80-20", "050034"), PropertyType.Apartment, TransactionType.Rent, new Price(1300000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Apartamento económico cerca a universidades, muy bien cuidado con excelente vista. Apartamento en Robledo"));
                var p7 = SafeExtract(Property.Create(new Address("Medellín", "Castilla", "Cra 65 #90-15", "050010"), PropertyType.House, TransactionType.Rent, new Price(1400000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Casa tradicional antioqueña, muy amplia, 3 habitaciones, patio grande con jardín. Casa tradicional en Castilla"));
                var p8 = SafeExtract(Property.Create(new Address("Medellín", "Aranjuez", "Calle 92 #55-30", "050010"), PropertyType.Studio, TransactionType.Rent, new Price(1200000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Estudio moderno perfecto para estudiantes o profesionales, excelente ubicación y precio justo. Estudio en Aranjuez"));
                var p9 = SafeExtract(Property.Create(new Address("Medellín", "Buenos Aires", "Cra 33 #22-15", "050013"), PropertyType.Apartment, TransactionType.Rent, new Price(1750000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Apartamento luminoso con vista privilegiada a la ciudad desde las laderas de Buenos Aires. Apartamento en Buenos Aires"));

                p1.AddImage("/images/properties/apt1.png");
                p2.AddImage("/images/properties/house1.png");
                p3.AddImage("/images/properties/studio1.png");
                p4.AddImage("/images/properties/house1.png");
                p5.AddImage("/images/properties/apt1.png");
                p6.AddImage("/images/properties/apt1.png");
                p7.AddImage("/images/properties/house1.png");
                p8.AddImage("/images/properties/studio1.png");
                p9.AddImage("/images/properties/apt1.png");

                var properties = new[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 };

                // Verificar todas las propiedades para que aparezcan en búsquedas públicas
                foreach (var prop in properties)
                    prop.MarkAsVerified();

                context.Properties.AddRange(properties);
                await context.SaveChangesAsync();
            }
        }
    }
}
