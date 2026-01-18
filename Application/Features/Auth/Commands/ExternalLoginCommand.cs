using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to authenticate via external provider (Google).
/// </summary>
public record ExternalLoginCommand : ICommand<Result<AuthResult>>
{
    public string Provider { get; init; } = string.Empty;
    public string IdToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? DeviceInfo { get; init; }
}

public class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required")
            .Must(x => x.Equals("Google", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only Google provider is currently supported");

        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("ID token is required");
    }
}

public class ExternalLoginCommandHandler : ICommandHandler<ExternalLoginCommand, Result<AuthResult>>
{
    private readonly IAuthService _authService;
    private readonly IExternalAuthService _externalAuthService;

    public ExternalLoginCommandHandler(
        IAuthService authService,
        IExternalAuthService externalAuthService)
    {
        _authService = authService;
        _externalAuthService = externalAuthService;
    }

    public async Task<Result<AuthResult>> Handle(
        ExternalLoginCommand command,
        CancellationToken cancellationToken)
    {
        // Validate the external token and get user info
        var externalInfo = await _externalAuthService.ValidateExternalTokenAsync(
            command.Provider,
            command.IdToken,
            cancellationToken);

        if (externalInfo == null)
        {
            return Result<AuthResult>.Unauthorized("Invalid external token");
        }

        // Authenticate or create user
        var result = await _authService.ExternalLoginAsync(
            externalInfo,
            command.IpAddress,
            command.DeviceInfo,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result<AuthResult>.Failure(result.Error!, result.StatusCode);
        }

        return Result<AuthResult>.Success(result);
    }
}
