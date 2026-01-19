using Application.Core;
using Application.Features.Users.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Users.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
public record CreateUserCommand : ICommand<Result<string>>
{
    public required CreateUserDto UserDto { get; init; }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserDto.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.UserDto.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

        RuleFor(x => x.UserDto.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

        RuleFor(x => x.UserDto.DisplayName)
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.UserDto.DisplayName));
    }
}

public class CreateUserCommandHandler(
    IUserService userService) : ICommandHandler<CreateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await userService.CreateUserAsync(
            command.UserDto.Email,
            command.UserDto.UserName,
            command.UserDto.DisplayName,
            command.UserDto.Password);

        return result;
    }
}
