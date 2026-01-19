namespace Application.Contracts.Identity;

/// <summary>
/// Internal result from auth service containing both tokens.
/// The refresh token is included here but should be converted to
/// an HttpOnly cookie by the API layer, NOT sent in the response body.
/// </summary>
public record AuthResult
{
    public bool Succeeded { get; init; }
    public AuthUserDto? User { get; init; }
    public string? AccessToken { get; init; }
    public DateTime AccessTokenExpiration { get; init; }

    /// <summary>
    /// The plain-text refresh token (will be hashed before storage).
    /// This should be set as an HttpOnly cookie, NOT returned to the client in JSON.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    public DateTime RefreshTokenExpiration { get; init; }

    public string? Error { get; init; }
    public int StatusCode { get; init; } = 200;

    public static AuthResult Success(
        AuthUserDto user,
        string accessToken,
        DateTime accessTokenExpiration,
        string refreshToken,
        DateTime refreshTokenExpiration) => new()
        {
            Succeeded = true,
            User = user,
            AccessToken = accessToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration
        };

    public static AuthResult Failure(string error, int statusCode = 400) => new()
    {
        Succeeded = false,
        Error = error,
        StatusCode = statusCode
    };
}
