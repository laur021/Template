using Application.Core;
using Application.Features.Users.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Users.Commands;

/// <summary>
/// Command to edit an existing user.
/// </summary>
public record EditUserCommand : ICommand<Result>
{
    public required EditUserDto UserDto { get; init; }
}

public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
{
    public EditUserCommandValidator()
    {
        RuleFor(x => x.UserDto.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.UserDto.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.UserDto.DisplayName)
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.UserDto.DisplayName));

        RuleFor(x => x.UserDto.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.UserDto.Bio));
    }
}

public class EditUserCommandHandler(
    IUserService userService) : ICommandHandler<EditUserCommand, Result>
{
    public async Task<Result> Handle(
        EditUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await userService.UpdateUserAsync(
            command.UserDto.Id,
            command.UserDto.Email,
            command.UserDto.DisplayName,
            command.UserDto.Bio);

        return result;
    }
}
