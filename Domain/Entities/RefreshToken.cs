namespace Domain.Entities;

/// <summary>
/// Domain entity representing a refresh token.
/// Refresh tokens enable silent re-authentication without re-entering credentials.
///
/// SECURITY NOTES:
/// - Token is stored as a HASH in the database, never in plain text
/// - Each token can only be used once (rotation on every refresh)
/// - Tokens have an expiration date (typically 7-30 days)
/// - Tokens can be explicitly revoked (logout, security breach)
/// - Multiple devices = multiple refresh tokens per user
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }

    /// <summary>
    /// The hashed value of the refresh token.
    /// We store a hash because if the database is compromised,
    /// attackers cannot use the hashed values to authenticate.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The user this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// When the token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the token expires. After this time, the user must re-authenticate.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the token was revoked (if applicable).
    /// A revoked token cannot be used even if not expired.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// The token that replaced this one during rotation.
    /// Used for security auditing and detecting token reuse attacks.
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>
    /// Device/browser identifier for this token.
    /// Helps users manage their sessions across devices.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// IP address from which the token was created.
    /// Used for security auditing.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Checks if this token is currently active (not expired and not revoked).
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}
