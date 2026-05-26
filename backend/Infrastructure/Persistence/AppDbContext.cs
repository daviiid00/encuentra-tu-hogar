using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Infrastructure.Identity;
using EncuentraTuHogar.Domain.ValueObjects;
using System.Text.Json;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Data Protection keys — persisten entre reinicios/redeploys
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public DbSet<Property> Properties { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<CommunityPost> CommunityPosts { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Property ─────────────────────────────────────────────────────────
        builder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasConversion(
                      id => id.Value,
                      value => PropertyId.From(value));

            entity.OwnsOne(e => e.Address, a =>
            {
                a.Property(x => x.City).HasMaxLength(100).IsRequired();
                a.Property(x => x.Zone).HasMaxLength(100).IsRequired();
                a.Property(x => x.Street).HasMaxLength(200);
                a.Property(x => x.PostalCode).HasMaxLength(20);
            });

            entity.OwnsOne(e => e.Price, p =>
            {
                p.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                p.Property(x => x.Currency).HasMaxLength(10);
            });

            entity.Property(e => e.OwnerId)
                  .HasConversion(
                      id => id.Value.ToString(),
                      value => UserId.From(Guid.Parse(value)));

            entity.Property(e => e.ImageUrls)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                      v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)
                           ?? new List<string>())
                  .Metadata.SetValueComparer(
                      new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                          (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
                          c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                          c => c.ToList()));

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // ── Visit ─────────────────────────────────────────────────────────────
        builder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitorId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // ── Review ────────────────────────────────────────────────────────────
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReviewerId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();

            // One review per visit
            entity.HasIndex(e => e.VisitId).IsUnique();
        });

        // ── CommunityPost ─────────────────────────────────────────────────────
        builder.Entity<CommunityPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AuthorId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(5000).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // ── Conversation ──────────────────────────────────────────────────────
        builder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ParticipantAId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.ParticipantBId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(e => e.Messages)
                  .WithOne(m => m.Conversation)
                  .HasForeignKey(m => m.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ParticipantAId, e.ParticipantBId, e.PropertyId });
        });

        // ── Message ───────────────────────────────────────────────────────────
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SenderId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.SentAt).IsRequired();
        });
    }
}
