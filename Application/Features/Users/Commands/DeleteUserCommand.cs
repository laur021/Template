using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Users.Commands;

/// <summary>
/// Command to delete a user.
/// </summary>
public record DeleteUserCommand : ICommand<Result>
{
    public string Id { get; init; } = string.Empty;
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");
    }
}

public class DeleteUserCommandHandler(
    IUserService userService) : ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await userService.DeleteUserAsync(command.Id);
        return result;
    }
}
