namespace Application.Interfaces;

/// <summary>
/// JWT token generation service interface.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate a JWT access token for the user.
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <param name="email">User's email address</param>
    /// <param name="roles">User's roles for authorization claims</param>
    /// <returns>JWT token string and expiration time</returns>
    (string Token, DateTime Expiration) GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles);

    /// <summary>
    /// Generate a cryptographically secure refresh token.
    /// </summary>
    /// <returns>Random token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Hash a refresh token for secure storage.
    /// </summary>
    string HashToken(string token);
}
