using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Infrastructure.Identity;
using EncuentraTuHogar.Domain.ValueObjects;
using System.Text.Json;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Property> Properties { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                  .HasConversion(id => id.Value, value => PropertyId.From(value));
                  
            entity.OwnsOne(e => e.Address);
            entity.OwnsOne(e => e.Price);
            
            entity.Property(e => e.OwnerId)
                  .HasConversion(id => id.Value.ToString(), value => UserId.From(System.Guid.Parse(value)));
                  
            entity.Property(e => e.ImageUrls)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                      v => JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(v, (JsonSerializerOptions)null!) ?? new System.Collections.Generic.List<string>()
                  );
        });
        
        builder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
