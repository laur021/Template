using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to confirm a user's email address.
/// </summary>
public record ConfirmEmailCommand : ICommand<Result>
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Confirmation token is required");
    }
}

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, Result>
{
    private readonly IAuthService _authService;

    public ConfirmEmailCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(
            command.Email,
            command.Token,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Error!, result.StatusCode);
        }

        return Result.Success();
    }
}
