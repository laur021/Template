using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Application database context extending IdentityDbContext for authentication.
/// Uses Guid as the primary key type for all Identity entities.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Refresh tokens for JWT authentication.
    /// </summary>
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure AppUser
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.DisplayName).HasMaxLength(100);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.ImageUrl).HasMaxLength(500);

            // Index for faster lookup by email
            entity.HasIndex(u => u.Email);
        });

        // Configure RefreshToken
        builder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(rt => rt.DeviceInfo)
                .HasMaxLength(500);

            entity.Property(rt => rt.IpAddress)
                .HasMaxLength(50);

            // Index on TokenHash for fast lookup during refresh
            entity.HasIndex(rt => rt.TokenHash);

            // Index on UserId for finding all tokens for a user
            entity.HasIndex(rt => rt.UserId);

            // Relationship with AppUser
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
