using Application.Contracts.Identity;
using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AuthController : BaseApiController
{
    private const string RefreshTokenCookieName = "refresh_token";

    // ======================================
    // Public Endpoints (No Authentication)
    // ======================================

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        return HandleResult(await Mediator.SendCommandAsync<RegisterUserCommand, Application.Core.Result<AuthResult>>(
            new RegisterUserCommand { Email = request.Email, Password = request.Password, DisplayName = request.DisplayName, ConfirmationUrlBase = request.ConfirmationUrlBase }));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await Mediator.SendCommandAsync<LoginUserCommand, Application.Core.Result<AuthResult>>(
            new LoginUserCommand { Email = request.Email, Password = request.Password, IpAddress = GetClientIpAddress(), DeviceInfo = GetDeviceInfo() });

        if (!result.IsSuccess) return HandleResult(result);
        SetRefreshTokenCookie(result.Value!);
        return Ok(ToAuthResponse(result.Value!));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var result = await Mediator.SendCommandAsync<RefreshTokenCommand, Application.Core.Result<AuthResult>>(
            new RefreshTokenCommand { RefreshToken = Request.Cookies[RefreshTokenCookieName] ?? string.Empty, IpAddress = GetClientIpAddress(), DeviceInfo = GetDeviceInfo() });

        if (!result.IsSuccess) return HandleResult(result);
        SetRefreshTokenCookie(result.Value!);
        return Ok(ToAuthResponse(result.Value!));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await Mediator.SendCommandAsync<LogoutCommand, Application.Core.Result>(new LogoutCommand { RefreshToken = Request.Cookies[RefreshTokenCookieName] ?? string.Empty });
        ClearRefreshTokenCookie();
        return Ok(new { Message = "Logged out successfully" });
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        return HandleResult(await Mediator.SendCommandAsync<ConfirmEmailCommand, Application.Core.Result>(
            new ConfirmEmailCommand { Email = request.Email, Token = request.Token }));
    }

    [AllowAnonymous]
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        await Mediator.SendCommandAsync<ResendEmailConfirmationCommand, Application.Core.Result>(
            new ResendEmailConfirmationCommand { Email = request.Email, ConfirmationUrlBase = request.ConfirmationUrlBase });
        return Ok(new { Message = "If the email exists, a confirmation link has been sent" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await Mediator.SendCommandAsync<ForgotPasswordCommand, Application.Core.Result>(
            new ForgotPasswordCommand { Email = request.Email, ResetUrlBase = request.ResetUrlBase });
        return Ok(new { Message = "If the email exists, a password reset link has been sent" });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        return HandleResult(await Mediator.SendCommandAsync<ResetPasswordCommand, Application.Core.Result>(
            new ResetPasswordCommand { Email = request.Email, Token = request.Token, NewPassword = request.NewPassword }));
    }

    [AllowAnonymous]
    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin(ExternalLoginRequest request)
    {
        var result = await Mediator.SendCommandAsync<ExternalLoginCommand, Application.Core.Result<AuthResult>>(
            new ExternalLoginCommand { Provider = request.Provider, IdToken = request.IdToken, IpAddress = GetClientIpAddress(), DeviceInfo = GetDeviceInfo() });

        if (!result.IsSuccess) return HandleResult(result);
        SetRefreshTokenCookie(result.Value!);
        return Ok(ToAuthResponse(result.Value!));
    }

    // ======================================
    // Protected Endpoints (Authentication Required)
    // ======================================

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        return HandleResult(await Mediator.SendCommandAsync<ChangePasswordCommand, Application.Core.Result>(
            new ChangePasswordCommand { CurrentPassword = request.CurrentPassword, NewPassword = request.NewPassword }));
    }

    // ======================================
    // Helper Methods
    // ======================================

    /// <summary>
    /// Set the refresh token as an HttpOnly cookie.
    ///
    /// SECURITY NOTES:
    /// - HttpOnly: Cookie cannot be accessed via JavaScript (XSS protection)
    /// - Secure: Cookie only sent over HTTPS (prevents interception)
    /// - SameSite=Strict: Cookie not sent with cross-site requests (CSRF protection)
    /// - Path=/api/auth: Cookie only sent to auth endpoints (limits exposure)
    /// </summary>
    private void SetRefreshTokenCookie(AuthResult authResult)
    {
        if (string.IsNullOrEmpty(authResult.RefreshToken))
            return;

        var cookieOptions = new CookieOptions
        {
            // HttpOnly: Prevents JavaScript access - critical for security
            // If an XSS vulnerability exists, attackers cannot steal the refresh token
            HttpOnly = true,

            // Secure: Only send cookie over HTTPS
            // Prevents token interception on insecure connections
            Secure = true,

            // SameSite=Strict: Cookie not sent with any cross-site requests
            // Provides strong CSRF protection
            // Note: For some scenarios, SameSite=Lax may be needed
            SameSite = SameSiteMode.Strict,

            // Expiration matches the refresh token's expiration
            Expires = authResult.RefreshTokenExpiration,

            // Path: Limit cookie to auth endpoints only
            // Reduces cookie exposure to other endpoints
            Path = "/api/auth"
        };

        Response.Cookies.Append(
            RefreshTokenCookieName,
            authResult.RefreshToken,
            cookieOptions);
    }

    /// <summary>
    /// Clear the refresh token cookie (used on logout or invalid token).
    /// </summary>
    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        });
    }

    /// <summary>
    /// Convert AuthResult to AuthResponse (excludes refresh token from response body).
    /// The refresh token is only sent as an HttpOnly cookie, never in JSON.
    /// </summary>
    private static AuthResponse ToAuthResponse(AuthResult authResult)
    {
        return new AuthResponse
        {
            User = authResult.User!,
            AccessToken = authResult.AccessToken!,
            AccessTokenExpiration = authResult.AccessTokenExpiration
        };
    }

    /// <summary>
    /// Get client IP address from request headers or connection.
    /// </summary>
    private string? GetClientIpAddress()
    {
        // Check for forwarded header (when behind reverse proxy)
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Get device/browser information from User-Agent header.
    /// </summary>
    private string? GetDeviceInfo()
    {
        return Request.Headers.UserAgent.FirstOrDefault();
    }
}

// ======================================
// Request DTOs
// ======================================

public record RegisterRequest(
    string Email,
    string Password,
    string? DisplayName,
    string ConfirmationUrlBase);

public record LoginRequest(
    string Email,
    string Password);

public record ConfirmEmailRequest(
    string Email,
    string Token);

public record ResendConfirmationRequest(
    string Email,
    string ConfirmationUrlBase);

public record ForgotPasswordRequest(
    string Email,
    string ResetUrlBase);

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);

public record ExternalLoginRequest(
    string Provider,
    string IdToken);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
