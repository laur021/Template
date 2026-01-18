namespace Infrastructure.Identity;

/// <summary>
/// Entity for storing refresh tokens in the database.
/// Maps to Domain.Entities.RefreshToken but includes EF Core navigation.
/// </summary>
public class RefreshTokenEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// The hashed value of the refresh token.
    /// SECURITY: We store a hash because if the database is compromised,
    /// attackers cannot use the hashed values to authenticate.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public AppUser User { get; set; } = null!;

    /// <summary>
    /// When the token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the token was revoked (null if still active).
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// The token ID that replaced this one during rotation.
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>
    /// Device/browser information for this session.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// IP address that created this token.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Check if the token is currently active.
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}
