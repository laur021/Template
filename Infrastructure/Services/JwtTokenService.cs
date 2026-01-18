using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

/// <summary>
/// Service for generating and managing JWT tokens.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Generate a JWT access token containing user claims.
    ///
    /// SECURITY NOTES:
    /// - Token is signed with HMAC-SHA256 using a secret key
    /// - Contains user ID, email, and roles as claims
    /// - Has a short expiration time to limit exposure
    /// - Should be stored in memory only (not localStorage) on the client
    /// </summary>
    public (string Token, DateTime Expiration) GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            // Subject claim (user ID) - standard JWT claim
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),

            // Email claim
            new(JwtRegisteredClaimNames.Email, email),

            // Unique token identifier - helps prevent token replay
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Issued at timestamp
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }

    /// <summary>
    /// Generate a cryptographically secure random refresh token.
    ///
    /// SECURITY NOTES:
    /// - Uses cryptographic random number generator
    /// - 64 bytes = 512 bits of entropy (highly secure)
    /// - Base64 encoded for safe storage and transmission
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Hash a token for secure database storage.
    ///
    /// SECURITY NOTES:
    /// - SHA256 is one-way - cannot recover original token from hash
    /// - If database is breached, hashed tokens are useless to attackers
    /// - Attacker would need the original token (stored only in HttpOnly cookie)
    /// </summary>
    public string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
