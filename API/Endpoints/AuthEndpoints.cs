using Application.Contracts.Identity;
using Application.Core;
using Application.Features.Auth.Commands;
using Application.Features.Auth.Queries;
using Cortex.Mediator;

namespace API.Endpoints;

/// <summary>
/// Authentication endpoints using Minimal API pattern.
/// All endpoints are thin - they only map HTTP requests to CQRS commands/queries.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Name of the HttpOnly cookie used to store the refresh token.
    /// </summary>
    private const string RefreshTokenCookieName = "refreshToken";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // ======================================
        // Public Endpoints (No Authentication Required)
        // ======================================

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user account")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Login with email and password")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token using refresh token cookie")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithSummary("Logout and revoke refresh token")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK);

        group.MapPost("/confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithSummary("Confirm email address with token")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/resend-confirmation", ResendConfirmation)
            .WithName("ResendConfirmation")
            .WithSummary("Resend email confirmation link")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK);

        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithSummary("Send password reset email")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK);

        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithSummary("Reset password with token from email")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/external-login", ExternalLogin)
            .WithName("ExternalLogin")
            .WithSummary("Login with external provider (Google)")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        // ======================================
        // Protected Endpoints (Authentication Required)
        // ======================================

        group.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .WithSummary("Change password for authenticated user")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    // ======================================
    // Endpoint Handlers
    // ======================================

    private static async Task<IResult> Register(
        RegisterRequest request,
        IMediator mediator,
        HttpContext httpContext)
    {
        var command = new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.DisplayName,
            IpAddress = GetClientIpAddress(httpContext),
            DeviceInfo = GetDeviceInfo(httpContext),
            ConfirmationUrlBase = request.ConfirmationUrlBase
        };

        var result = await mediator.SendCommandAsync<RegisterUserCommand, Result<AuthResult>>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        // Set refresh token as HttpOnly cookie
        SetRefreshTokenCookie(httpContext, result.Value!);

        return Results.Ok(ToAuthResponse(result.Value!));
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IMediator mediator,
        HttpContext httpContext)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            IpAddress = GetClientIpAddress(httpContext),
            DeviceInfo = GetDeviceInfo(httpContext)
        };

        var result = await mediator.SendCommandAsync<LoginUserCommand, Result<AuthResult>>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        // Set refresh token as HttpOnly cookie
        SetRefreshTokenCookie(httpContext, result.Value!);

        return Results.Ok(ToAuthResponse(result.Value!));
    }

    private static async Task<IResult> RefreshToken(
        IMediator mediator,
        HttpContext httpContext)
    {
        // Read refresh token from HttpOnly cookie
        var refreshToken = httpContext.Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Problem(
                title: "Unauthorized",
                detail: "No refresh token provided",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshToken,
            IpAddress = GetClientIpAddress(httpContext),
            DeviceInfo = GetDeviceInfo(httpContext)
        };

        var result = await mediator.SendCommandAsync<RefreshTokenCommand, Result<AuthResult>>(command);

        if (!result.IsSuccess)
        {
            // Clear the invalid cookie
            ClearRefreshTokenCookie(httpContext);
            return ToProblemResult(result);
        }

        // Set new refresh token cookie (token rotation)
        SetRefreshTokenCookie(httpContext, result.Value!);

        return Results.Ok(ToAuthResponse(result.Value!));
    }

    private static async Task<IResult> Logout(
        IMediator mediator,
        HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies[RefreshTokenCookieName];

        var command = new LogoutCommand
        {
            RefreshToken = refreshToken ?? string.Empty
        };

        await mediator.SendCommandAsync<LogoutCommand, Result>(command);

        // Always clear the cookie, even if token wasn't found
        ClearRefreshTokenCookie(httpContext);

        return Results.Ok(new { Message = "Logged out successfully" });
    }

    private static async Task<IResult> ConfirmEmail(
        ConfirmEmailRequest request,
        IMediator mediator)
    {
        var command = new ConfirmEmailCommand
        {
            Email = request.Email,
            Token = request.Token
        };

        var result = await mediator.SendCommandAsync<ConfirmEmailCommand, Result>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        return Results.Ok(new { Message = "Email confirmed successfully" });
    }

    private static async Task<IResult> ResendConfirmation(
        ResendConfirmationRequest request,
        IMediator mediator)
    {
        var command = new ResendEmailConfirmationCommand
        {
            Email = request.Email,
            ConfirmationUrlBase = request.ConfirmationUrlBase
        };

        await mediator.SendCommandAsync<ResendEmailConfirmationCommand, Result>(command);

        // Always return success to prevent email enumeration
        return Results.Ok(new { Message = "If the email exists, a confirmation link has been sent" });
    }

    private static async Task<IResult> ForgotPassword(
        ForgotPasswordRequest request,
        IMediator mediator)
    {
        var command = new ForgotPasswordCommand
        {
            Email = request.Email,
            ResetUrlBase = request.ResetUrlBase
        };

        await mediator.SendCommandAsync<ForgotPasswordCommand, Result>(command);

        // Always return success to prevent email enumeration
        return Results.Ok(new { Message = "If the email exists, a password reset link has been sent" });
    }

    private static async Task<IResult> ResetPassword(
        ResetPasswordRequest request,
        IMediator mediator)
    {
        var command = new ResetPasswordCommand
        {
            Email = request.Email,
            Token = request.Token,
            NewPassword = request.NewPassword
        };

        var result = await mediator.SendCommandAsync<ResetPasswordCommand, Result>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        return Results.Ok(new { Message = "Password reset successfully" });
    }

    private static async Task<IResult> ExternalLogin(
        ExternalLoginRequest request,
        IMediator mediator,
        HttpContext httpContext)
    {
        var command = new ExternalLoginCommand
        {
            Provider = request.Provider,
            IdToken = request.IdToken,
            IpAddress = GetClientIpAddress(httpContext),
            DeviceInfo = GetDeviceInfo(httpContext)
        };

        var result = await mediator.SendCommandAsync<ExternalLoginCommand, Result<AuthResult>>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        // Set refresh token as HttpOnly cookie
        SetRefreshTokenCookie(httpContext, result.Value!);

        return Results.Ok(ToAuthResponse(result.Value!));
    }

    private static async Task<IResult> ChangePassword(
        ChangePasswordRequest request,
        IMediator mediator)
    {
        var command = new ChangePasswordCommand
        {
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };

        var result = await mediator.SendCommandAsync<ChangePasswordCommand, Result>(command);

        if (!result.IsSuccess)
        {
            return ToProblemResult(result);
        }

        return Results.Ok(new { Message = "Password changed successfully" });
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
    private static void SetRefreshTokenCookie(HttpContext httpContext, AuthResult authResult)
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

        httpContext.Response.Cookies.Append(
            RefreshTokenCookieName,
            authResult.RefreshToken,
            cookieOptions);
    }

    /// <summary>
    /// Clear the refresh token cookie (used on logout or invalid token).
    /// </summary>
    private static void ClearRefreshTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
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
    private static string? GetClientIpAddress(HttpContext httpContext)
    {
        // Check for forwarded header (when behind reverse proxy)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Get device/browser information from User-Agent header.
    /// </summary>
    private static string? GetDeviceInfo(HttpContext httpContext)
    {
        return httpContext.Request.Headers.UserAgent.FirstOrDefault();
    }

    /// <summary>
    /// Convert Result to ProblemDetails response.
    /// </summary>
    private static IResult ToProblemResult<T>(Result<T> result)
    {
        return Results.Problem(
            title: GetTitleForStatusCode(result.StatusCode),
            detail: result.Error,
            statusCode: result.StatusCode);
    }

    private static IResult ToProblemResult(Result result)
    {
        return Results.Problem(
            title: GetTitleForStatusCode(result.StatusCode),
            detail: result.Error,
            statusCode: result.StatusCode);
    }

    private static string GetTitleForStatusCode(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        _ => "Error"
    };
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
