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

    /// <summary>
    /// User profiles - extended user details (1:1 with AspNetUsers).
    /// </summary>
    public DbSet<UserProfileEntity> UserProfiles => Set<UserProfileEntity>();

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

        // Configure UserProfile (1:1 relationship with AppUser)
        builder.Entity<UserProfileEntity>(entity =>
        {
            entity.HasKey(up => up.Id);

            entity.Property(up => up.FirstName).HasMaxLength(100);
            entity.Property(up => up.LastName).HasMaxLength(100);
            entity.Property(up => up.DisplayName).HasMaxLength(100);
            entity.Property(up => up.Bio).HasMaxLength(1000);
            entity.Property(up => up.ImageUrl).HasMaxLength(500);
            entity.Property(up => up.PhoneNumber).HasMaxLength(20);
            entity.Property(up => up.Gender).HasMaxLength(20);
            entity.Property(up => up.Address).HasMaxLength(200);
            entity.Property(up => up.City).HasMaxLength(100);
            entity.Property(up => up.State).HasMaxLength(100);
            entity.Property(up => up.Country).HasMaxLength(100);
            entity.Property(up => up.PostalCode).HasMaxLength(20);

            // 1:1 relationship with AppUser (AspNetUsers)
            // The Id is both PK and FK to AspNetUsers
            entity.HasOne(up => up.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfileEntity>(up => up.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
