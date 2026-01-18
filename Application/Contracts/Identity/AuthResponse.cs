namespace Application.Contracts.Identity;

/// <summary>
/// Response returned after successful authentication.
/// Contains the short-lived access token.
/// The refresh token is NOT included here - it's set as an HttpOnly cookie
/// by the API layer for security reasons.
/// </summary>
public record AuthResponse
{
    /// <summary>
    /// The authenticated user's information.
    /// </summary>
    public UserDto User { get; init; } = null!;

    /// <summary>
    /// JWT access token for API authorization.
    /// This token is SHORT-LIVED (5-15 minutes) and should be stored
    /// in memory only (not localStorage/sessionStorage for XSS protection).
    /// Angular app should store this in a service/state management.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// When the access token expires (UTC).
    /// Angular can use this to proactively refresh before expiration.
    /// </summary>
    public DateTime AccessTokenExpiration { get; init; }
}
