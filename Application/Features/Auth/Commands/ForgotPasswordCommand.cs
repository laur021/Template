using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to initiate the forgot password flow.
/// Sends a password reset email to the user.
/// </summary>
public record ForgotPasswordCommand : ICommand<Result>
{
    public string Email { get; init; } = string.Empty;
    public string ResetUrlBase { get; init; } = string.Empty;
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.ResetUrlBase)
            .NotEmpty().WithMessage("Reset URL is required");
    }
}

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, Result>
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        // Always return success to prevent email enumeration attacks
        // Even if the email doesn't exist, we don't want to reveal that
        await _authService.ForgotPasswordAsync(
            command.Email,
            command.ResetUrlBase,
            cancellationToken);

        return Result.Success();
    }
}
