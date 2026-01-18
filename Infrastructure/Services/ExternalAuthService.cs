using Application.Contracts.Identity;
using Application.Interfaces;
using Google.Apis.Auth;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Service for validating external authentication tokens (Google).
/// </summary>
public class ExternalAuthService : IExternalAuthService
{
    private readonly GoogleAuthSettings _googleSettings;
    private readonly ILogger<ExternalAuthService> _logger;

    public ExternalAuthService(
        IOptions<GoogleAuthSettings> googleSettings,
        ILogger<ExternalAuthService> logger)
    {
        _googleSettings = googleSettings.Value;
        _logger = logger;
    }

    public async Task<ExternalAuthInfo?> ValidateExternalTokenAsync(
        string provider,
        string idToken,
        CancellationToken cancellationToken = default)
    {
        return provider.ToLowerInvariant() switch
        {
            "google" => await ValidateGoogleTokenAsync(idToken, cancellationToken),
            _ => null
        };
    }

    private async Task<ExternalAuthInfo?> ValidateGoogleTokenAsync(
        string idToken,
        CancellationToken cancellationToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleSettings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            if (payload == null)
            {
                _logger.LogWarning("Google token validation returned null payload");
                return null;
            }

            return new ExternalAuthInfo
            {
                Provider = "Google",
                ProviderKey = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
                ImageUrl = payload.Picture
            };
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google JWT token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google token");
            return null;
        }
    }
}
