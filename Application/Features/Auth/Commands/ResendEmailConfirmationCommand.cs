using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to resend the email confirmation link.
/// </summary>
public record ResendEmailConfirmationCommand : ICommand<Result>
{
    public string Email { get; init; } = string.Empty;
    public string ConfirmationUrlBase { get; init; } = string.Empty;
}

public class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.ConfirmationUrlBase)
            .NotEmpty().WithMessage("Confirmation URL is required");
    }
}

public class ResendEmailConfirmationCommandHandler : ICommandHandler<ResendEmailConfirmationCommand, Result>
{
    private readonly IAuthService _authService;

    public ResendEmailConfirmationCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(ResendEmailConfirmationCommand command, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendEmailConfirmationAsync(
            command.Email,
            command.ConfirmationUrlBase,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Error!, result.StatusCode);
        }

        return Result.Success();
    }
}
