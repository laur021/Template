using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to logout a user by revoking their refresh token.
/// </summary>
public record LogoutCommand : ICommand<Result>
{
    /// <summary>
    /// The refresh token to revoke (from HttpOnly cookie).
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
}

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            // No token to revoke, still return success
            return Result.Success();
        }

        var result = await _authService.LogoutAsync(command.RefreshToken, cancellationToken);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Error!, result.StatusCode);
        }

        return Result.Success();
    }
}
