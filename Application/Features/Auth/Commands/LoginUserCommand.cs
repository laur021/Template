using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Auth.Commands;

/// <summary>
/// Command to login a user with email and password.
/// </summary>
public record LoginUserCommand : ICommand<Result<AuthResult>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? DeviceInfo { get; init; }
}

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, Result<AuthResult>>
{
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResult>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            command.Email,
            command.Password,
            command.IpAddress,
            command.DeviceInfo,
            cancellationToken);

        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                401 => Result<AuthResult>.Unauthorized(result.Error!),
                403 => Result<AuthResult>.Forbidden(result.Error!),
                _ => Result<AuthResult>.Failure(result.Error!, result.StatusCode)
            };
        }

        return Result<AuthResult>.Success(result);
    }
}
