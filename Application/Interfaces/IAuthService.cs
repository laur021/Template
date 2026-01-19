using Application.Contracts.Identity;

namespace Application.Interfaces;

/// <summary>
/// Authentication service interface.
/// This abstraction allows the Application layer to use authentication
/// without depending on ASP.NET Identity directly.
/// The implementation in Infrastructure uses UserManager, SignInManager, etc.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user with email and password.
    /// </summary>
    Task<AuthResult> RegisterAsync(
        string email,
        string password,
        string? displayName,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticate user with email and password.
    /// </summary>
    Task<AuthResult> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh the access token using a valid refresh token.
    /// Implements token rotation (old token is invalidated, new one issued).
    /// </summary>
    Task<AuthResult> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout user by revoking the refresh token.
    /// </summary>
    Task<TokenResult> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all refresh tokens for a user (e.g., security breach).
    /// </summary>
    Task<TokenResult> RevokeAllTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticate or register user via external provider (Google).
    /// </summary>
    Task<AuthResult> ExternalLoginAsync(
        ExternalAuthInfo externalLogin,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate email confirmation token and send confirmation email.
    /// </summary>
    Task<EmailResult> SendEmailConfirmationAsync(
        string email,
        string confirmationUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm user's email with the provided token.
    /// </summary>
    Task<EmailResult> ConfirmEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resend email confirmation for a user.
    /// </summary>
    Task<EmailResult> ResendEmailConfirmationAsync(
        string email,
        string confirmationUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiate forgot password flow - send reset email.
    /// </summary>
    Task<EmailResult> ForgotPasswordAsync(
        string email,
        string resetUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password with the token from forgot password email.
    /// </summary>
    Task<EmailResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Change password for authenticated user.
    /// </summary>
    Task<EmailResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable or disable a user account.
    /// </summary>
    Task<EmailResult> SetUserEnabledAsync(
        Guid userId,
        bool enabled,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by ID.
    /// </summary>
    Task<AuthUserDto?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by email.
    /// </summary>
    Task<AuthUserDto?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}
