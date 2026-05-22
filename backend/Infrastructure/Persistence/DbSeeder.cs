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

            // 3. Properties
            if (!context.Properties.Any())
            {
                Property ExtractProperty(Result<Property> result) => ((Result<Property>.Success)result).Value;

                var p1 = ExtractProperty(Property.Create(new Address("Medellín", "El Poblado", "Cra 43A #1-50", "050022"), PropertyType.Apartment, TransactionType.Rent, new Price(2500000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Amplio apartamento con excelente vista, ideal para familias. Cuenta con zonas verdes y seguridad 24/7. Hermoso Apartamento en El Poblado"));
                var p2 = ExtractProperty(Property.Create(new Address("Envigado", "San José", "Calle 38 Sur # 43 - 20", "055422"), PropertyType.House, TransactionType.Rent, new Price(3200000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Hermosa casa de dos niveles con patio, 3 habitaciones y 2 baños. Casa familiar en Envigado"));
                var p3 = ExtractProperty(Property.Create(new Address("Medellín", "Laureles", "Circular 4 # 70-12", "050031"), PropertyType.Studio, TransactionType.Rent, new Price(1800000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Aptoestudio amoblado cerca de universidades, parques y comercio. Estudio moderno Laureles"));
                var p4 = ExtractProperty(Property.Create(new Address("Rionegro", "Llanogrande", "Vía San Antonio", "054040"), PropertyType.House, TransactionType.Rent, new Price(4500000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Vive rodeado de naturaleza a solo 40 minutos de la ciudad. 4 habitaciones, chimenea. Casa Campestre en Rionegro"));
                var p5 = ExtractProperty(Property.Create(new Address("Bogotá", "La Candelaria", "Calle 11 # 4-14", "111711"), PropertyType.Apartment, TransactionType.Rent, new Price(1600000m, "COP"), UserId.From(Guid.Parse(ownerUser.Id)), "Apartamento acogedor de 2 habitaciones, cerca de Transmilenio y zonas culturales. Apartamento centro de Bogotá"));

                var properties = new[] { p1, p2, p3, p4, p5 };

                context.Properties.AddRange(properties);
                await context.SaveChangesAsync();
            }
        }
    }
}
