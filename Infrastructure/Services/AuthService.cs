using Application.Contracts.Identity;
using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Authentication service implementation using ASP.NET Identity.
/// This service encapsulates all authentication logic and provides
/// a clean interface to the Application layer.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        IEmailService emailService,
        AppDbContext context,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(
        string email,
        string password,
        string? displayName,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return AuthResult.Failure("A user with this email already exists", 409);
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? email.Split('@')[0],
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User registration failed for {Email}: {Errors}", email, errors);
            return AuthResult.Failure(errors);
        }

        // Add default role
        await _userManager.AddToRoleAsync(user, "User");

        _logger.LogInformation("User {Email} registered successfully", email);

        // Generate tokens for auto-login after registration
        // Note: User still needs to confirm email before subsequent logins
        return await GenerateAuthResultAsync(user, ipAddress, deviceInfo, cancellationToken);
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            // Don't reveal that user doesn't exist (security)
            return AuthResult.Failure("Invalid email or password", 401);
        }

        // Check if account is disabled
        if (!user.IsActive)
        {
            return AuthResult.Failure("This account has been disabled", 403);
        }

        // Check if email is confirmed
        if (!user.EmailConfirmed)
        {
            return AuthResult.Failure("Please confirm your email address before logging in", 403);
        }

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Locked out user {Email} attempted login", email);
            return AuthResult.Failure("Account is locked. Please try again later.", 403);
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} has been locked out due to failed attempts", email);
                return AuthResult.Failure("Account has been locked due to too many failed attempts. Please try again later.", 403);
            }

            _logger.LogWarning("Failed login attempt for {Email}", email);
            return AuthResult.Failure("Invalid email or password", 401);
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {Email} logged in successfully", email);

        return await GenerateAuthResultAsync(user, ipAddress, deviceInfo, cancellationToken);
    }

    public async Task<AuthResult> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default)
    {
        // Hash the incoming token to compare with stored hash
        var tokenHash = _jwtTokenService.HashToken(refreshToken);

        // Find the token in database
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken == null)
        {
            _logger.LogWarning("Refresh token not found");
            return AuthResult.Failure("Invalid refresh token", 401);
        }

        // Check if token is still active
        if (!storedToken.IsActive)
        {
            // SECURITY: If someone tries to use a revoked token, this could indicate
            // token theft. Consider revoking all tokens for this user.
            if (storedToken.RevokedAt != null)
            {
                _logger.LogWarning(
                    "Attempted reuse of revoked refresh token for user {UserId}. Possible token theft.",
                    storedToken.UserId);

                // Revoke all tokens for this user as a security measure
                await RevokeAllTokensAsync(storedToken.UserId, cancellationToken);
            }

            return AuthResult.Failure("Refresh token has expired or been revoked", 401);
        }

        // Check if user is still active
        var user = storedToken.User;
        if (!user.IsActive)
        {
            return AuthResult.Failure("User account is disabled", 403);
        }

        // SECURITY: Token Rotation
        // Revoke the old token and issue a new one.
        // This limits the window of opportunity if a token is stolen.
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var authResult = await GenerateAuthResultAsync(user, ipAddress, deviceInfo, cancellationToken);

        // Link old token to new one for audit trail
        var newTokenHash = _jwtTokenService.HashToken(authResult.RefreshToken!);
        var newToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == newTokenHash, cancellationToken);

        if (newToken != null)
        {
            storedToken.ReplacedByTokenId = newToken.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

        return authResult;
    }

    public async Task<TokenResult> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = _jwtTokenService.HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken == null)
        {
            // Token not found, but we still return success
            // (don't reveal whether token existed)
            return TokenResult.Success();
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged out, refresh token revoked");

        return TokenResult.Success();
    }

    public async Task<TokenResult> RevokeAllTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);

        return TokenResult.Success();
    }

    public async Task<AuthResult> ExternalLoginAsync(
        ExternalAuthInfo externalLogin,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken = default)
    {
        // Try to find user by external login
        var user = await _userManager.FindByLoginAsync(
            externalLogin.Provider,
            externalLogin.ProviderKey);

        if (user == null)
        {
            // Try to find user by email
            user = await _userManager.FindByEmailAsync(externalLogin.Email);

            if (user == null)
            {
                // Create new user
                user = new AppUser
                {
                    UserName = externalLogin.Email,
                    Email = externalLogin.Email,
                    EmailConfirmed = true, // External providers verify email
                    DisplayName = externalLogin.DisplayName ?? externalLogin.Email.Split('@')[0],
                    ImageUrl = externalLogin.ImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return AuthResult.Failure(errors);
                }

                await _userManager.AddToRoleAsync(user, "User");

                _logger.LogInformation(
                    "New user created via {Provider} external login: {Email}",
                    externalLogin.Provider, externalLogin.Email);
            }

            // Link external login to user
            var loginInfo = new UserLoginInfo(
                externalLogin.Provider,
                externalLogin.ProviderKey,
                externalLogin.Provider);

            var linkResult = await _userManager.AddLoginAsync(user, loginInfo);
            if (!linkResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to link {Provider} login to user {Email}",
                    externalLogin.Provider, externalLogin.Email);
            }
        }

        // Check if account is active
        if (!user.IsActive)
        {
            return AuthResult.Failure("This account has been disabled", 403);
        }

        // Update user info from external provider
        if (!string.IsNullOrEmpty(externalLogin.ImageUrl) && user.ImageUrl != externalLogin.ImageUrl)
        {
            user.ImageUrl = externalLogin.ImageUrl;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation(
            "User {Email} logged in via {Provider}",
            user.Email, externalLogin.Provider);

        return await GenerateAuthResultAsync(user, ipAddress, deviceInfo, cancellationToken);
    }

    public async Task<EmailResult> SendEmailConfirmationAsync(
        string email,
        string confirmationUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return EmailResult.Success();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var link = $"{confirmationUrl}&token={encodedToken}";

        await _emailService.SendEmailConfirmationAsync(email, link, cancellationToken);

        return EmailResult.Success();
    }

    public async Task<EmailResult> ConfirmEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return EmailResult.Failure("Invalid confirmation request", 400);
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return EmailResult.Failure(errors);
        }

        _logger.LogInformation("Email confirmed for user {Email}", email);

        return EmailResult.Success();
    }

    public async Task<EmailResult> ResendEmailConfirmationAsync(
        string email,
        string confirmationUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return EmailResult.Success();
        }

        if (user.EmailConfirmed)
        {
            return EmailResult.Failure("Email is already confirmed", 400);
        }

        return await SendEmailConfirmationAsync(email, confirmationUrl, cancellationToken);
    }

    public async Task<EmailResult> ForgotPasswordAsync(
        string email,
        string resetUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist (security)
            return EmailResult.Success();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var link = $"{resetUrl}?email={Uri.EscapeDataString(email)}&token={encodedToken}";

        await _emailService.SendPasswordResetAsync(email, link, cancellationToken);

        _logger.LogInformation("Password reset email sent to {Email}", email);

        return EmailResult.Success();
    }

    public async Task<EmailResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return EmailResult.Failure("Invalid reset request", 400);
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return EmailResult.Failure(errors);
        }

        // Revoke all refresh tokens after password reset for security
        await RevokeAllTokensAsync(user.Id, cancellationToken);

        _logger.LogInformation("Password reset successfully for user {Email}", email);

        return EmailResult.Success();
    }

    public async Task<EmailResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return EmailResult.Failure("User not found", 404);
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return EmailResult.Failure(errors);
        }

        _logger.LogInformation("Password changed for user {UserId}", userId);

        return EmailResult.Success();
    }

    public async Task<EmailResult> SetUserEnabledAsync(
        Guid userId,
        bool enabled,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return EmailResult.Failure("User not found", 404);
        }

        user.IsActive = enabled;
        await _userManager.UpdateAsync(user);

        if (!enabled)
        {
            // Revoke all refresh tokens when disabling account
            await RevokeAllTokensAsync(userId, cancellationToken);
        }

        _logger.LogInformation(
            "User {UserId} has been {Status}",
            userId, enabled ? "enabled" : "disabled");

        return EmailResult.Success();
    }

    public async Task<UserDto?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : MapToUserDto(user);
    }

    #region Private Helpers

    private async Task<AuthResult> GenerateAuthResultAsync(
        AppUser user,
        string? ipAddress,
        string? deviceInfo,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);

        // Generate access token
        var (accessToken, accessTokenExpiration) = _jwtTokenService.GenerateAccessToken(
            user.Id,
            user.Email!,
            roles);

        // Generate refresh token
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        // Store refresh token hash in database
        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshTokenExpiration,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(
            MapToUserDto(user),
            accessToken,
            accessTokenExpiration,
            refreshToken,
            refreshTokenExpiration);
    }

    private static UserDto MapToUserDto(AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            ImageUrl = user.ImageUrl,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    #endregion
}
