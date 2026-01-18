using Application.Contracts.Identity;

namespace Application.Interfaces;

/// <summary>
/// Service for validating external authentication tokens (Google, etc.)
/// </summary>
public interface IExternalAuthService
{
    /// <summary>
    /// Validate an external provider's ID token and extract user information.
    /// </summary>
    /// <param name="provider">The provider name (e.g., "Google")</param>
    /// <param name="idToken">The ID token from the provider</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External login info if valid, null otherwise</returns>
    Task<ExternalAuthInfo?> ValidateExternalTokenAsync(
        string provider,
        string idToken,
        CancellationToken cancellationToken = default);
}
