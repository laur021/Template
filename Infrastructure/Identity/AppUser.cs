using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Application user entity extending IdentityUser with Guid as the primary key.
/// This is the Identity-specific user class used for authentication.
///
/// NOTE: This class is in the Infrastructure layer because it depends on
/// ASP.NET Identity. The Domain layer has a separate User entity that
/// is completely framework-agnostic.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>
    /// User's display name (shown in UI).
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Short biography or description.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// URL to user's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// When the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last time the user logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Whether the user account is active (can be used to disable accounts).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to user's refresh tokens.
    /// </summary>
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();

    /// <summary>
    /// Navigation property to user's extended profile/details.
    /// 1:1 relationship - UserProfile stores additional user information.
    /// </summary>
    public UserProfileEntity? Profile { get; set; }
}
