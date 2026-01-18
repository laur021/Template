using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to register a new user.
/// </summary>
public record RegisterUserCommand : ICommand<Result<AuthResult>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? IpAddress { get; init; }
    public string? DeviceInfo { get; init; }

    /// <summary>
    /// Base URL for email confirmation (e.g., https://yourapp.com/confirm-email)
    /// The token and email will be appended as query params.
    /// </summary>
    public string ConfirmationUrlBase { get; init; } = string.Empty;
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Display name cannot exceed 100 characters");

        RuleFor(x => x.ConfirmationUrlBase)
            .NotEmpty().WithMessage("Confirmation URL is required");
    }
}

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<AuthResult>>
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResult>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(
            command.Email,
            command.Password,
            command.DisplayName,
            command.IpAddress,
            command.DeviceInfo,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result<AuthResult>.Failure(result.Error!, result.StatusCode);
        }

        // Send confirmation email
        var confirmationUrl = $"{command.ConfirmationUrlBase}?email={Uri.EscapeDataString(command.Email)}";
        await _authService.SendEmailConfirmationAsync(
            command.Email,
            confirmationUrl,
            cancellationToken);

        return Result<AuthResult>.Success(result);
    }
}
