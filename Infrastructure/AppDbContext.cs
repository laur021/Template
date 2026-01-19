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

    #region Menu System

    /// <summary>
    /// Menu sections (parent level navigation).
    /// </summary>
    public DbSet<MenuSectionEntity> MenuSections => Set<MenuSectionEntity>();

    /// <summary>
    /// Menu items (child level navigation).
    /// </summary>
    public DbSet<MenuItemEntity> MenuItems => Set<MenuItemEntity>();

    /// <summary>
    /// Menu sub-items (grandchild level navigation).
    /// </summary>
    public DbSet<MenuSubItemEntity> MenuSubItems => Set<MenuSubItemEntity>();

    /// <summary>
    /// Page actions (buttons/operations on pages).
    /// </summary>
    public DbSet<PageActionEntity> PageActions => Set<PageActionEntity>();

    /// <summary>
    /// Role-menu access permissions.
    /// </summary>
    public DbSet<RoleMenuAccessEntity> RoleMenuAccess => Set<RoleMenuAccessEntity>();

    /// <summary>
    /// Role-action access permissions.
    /// </summary>
    public DbSet<RoleActionAccessEntity> RoleActionAccess => Set<RoleActionAccessEntity>();

    #endregion

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

        #region Menu System Configuration

        // Configure MenuSection
        builder.Entity<MenuSectionEntity>(entity =>
        {
            entity.HasKey(ms => ms.Id);
            entity.Property(ms => ms.Name).IsRequired().HasMaxLength(100);
            entity.Property(ms => ms.Icon).HasMaxLength(50);
            entity.HasIndex(ms => ms.DisplayOrder);
        });

        // Configure MenuItem
        builder.Entity<MenuItemEntity>(entity =>
        {
            entity.HasKey(mi => mi.Id);
            entity.Property(mi => mi.Name).IsRequired().HasMaxLength(100);
            entity.Property(mi => mi.Icon).HasMaxLength(50);
            entity.Property(mi => mi.Route).HasMaxLength(200);
            entity.HasIndex(mi => mi.DisplayOrder);
            entity.HasIndex(mi => mi.Route);

            entity.HasOne(mi => mi.Section)
                .WithMany(s => s.MenuItems)
                .HasForeignKey(mi => mi.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MenuSubItem
        builder.Entity<MenuSubItemEntity>(entity =>
        {
            entity.HasKey(msi => msi.Id);
            entity.Property(msi => msi.Name).IsRequired().HasMaxLength(100);
            entity.Property(msi => msi.Icon).HasMaxLength(50);
            entity.Property(msi => msi.Route).IsRequired().HasMaxLength(200);
            entity.HasIndex(msi => msi.DisplayOrder);
            entity.HasIndex(msi => msi.Route);

            entity.HasOne(msi => msi.MenuItem)
                .WithMany(mi => mi.SubItems)
                .HasForeignKey(msi => msi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PageAction
        builder.Entity<PageActionEntity>(entity =>
        {
            entity.HasKey(pa => pa.Id);
            entity.Property(pa => pa.Code).IsRequired().HasMaxLength(50);
            entity.Property(pa => pa.Name).IsRequired().HasMaxLength(100);
            entity.Property(pa => pa.Description).HasMaxLength(500);
            entity.HasIndex(pa => pa.Code);

            entity.HasOne(pa => pa.MenuItem)
                .WithMany(mi => mi.Actions)
                .HasForeignKey(pa => pa.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pa => pa.MenuSubItem)
                .WithMany(msi => msi.Actions)
                .HasForeignKey(pa => pa.MenuSubItemId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configure RoleMenuAccess
        builder.Entity<RoleMenuAccessEntity>(entity =>
        {
            entity.HasKey(rma => rma.Id);

            // Composite index for faster lookups
            entity.HasIndex(rma => new { rma.RoleId, rma.SectionId, rma.MenuItemId, rma.SubItemId });

            entity.HasOne(rma => rma.Role)
                .WithMany()
                .HasForeignKey(rma => rma.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rma => rma.Section)
                .WithMany(s => s.RoleMenuAccess)
                .HasForeignKey(rma => rma.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rma => rma.MenuItem)
                .WithMany(mi => mi.RoleMenuAccess)
                .HasForeignKey(rma => rma.MenuItemId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(rma => rma.SubItem)
                .WithMany(msi => msi.RoleMenuAccess)
                .HasForeignKey(rma => rma.SubItemId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Configure RoleActionAccess
        builder.Entity<RoleActionAccessEntity>(entity =>
        {
            entity.HasKey(raa => raa.Id);

            // Composite index for faster lookups
            entity.HasIndex(raa => new { raa.RoleId, raa.ActionId }).IsUnique();

            entity.HasOne(raa => raa.Role)
                .WithMany()
                .HasForeignKey(raa => raa.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(raa => raa.Action)
                .WithMany(pa => pa.RoleActionAccess)
                .HasForeignKey(raa => raa.ActionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion
    }
}
