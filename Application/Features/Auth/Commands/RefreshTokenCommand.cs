using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to refresh the access token using a refresh token.
/// The refresh token is read from an HttpOnly cookie by the API layer.
/// </summary>
public record RefreshTokenCommand : ICommand<Result<AuthResult>>
{
    /// <summary>
    /// The refresh token from the HttpOnly cookie.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    public string? IpAddress { get; init; }
    public string? DeviceInfo { get; init; }
}

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<AuthResult>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return Result<AuthResult>.Unauthorized("No refresh token provided");
        }

        var result = await _authService.RefreshTokenAsync(
            command.RefreshToken,
            command.IpAddress,
            command.DeviceInfo,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result<AuthResult>.Unauthorized(result.Error ?? "Invalid refresh token");
        }

        return Result<AuthResult>.Success(result);
    }
}
