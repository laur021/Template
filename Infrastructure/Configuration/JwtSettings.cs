namespace Infrastructure.Configuration;

/// <summary>
/// Configuration settings for JWT authentication.
/// These are bound from appsettings.json.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Secret key for signing tokens. Should be at least 256 bits (32 characters).
    /// SECURITY: In production, use a secure secret stored in Azure Key Vault or similar.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer (who issued the token).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience (who the token is intended for).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token lifetime in minutes.
    /// SECURITY: Keep this short (5-15 minutes) to limit exposure if token is compromised.
    /// Users will use refresh tokens to get new access tokens.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token lifetime in days.
    /// SECURITY: This can be longer (7-30 days) because:
    /// 1. Stored as HttpOnly cookie (not accessible via JavaScript)
    /// 2. Rotated on every use (old token is invalidated)
    /// 3. Can be revoked server-side
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
