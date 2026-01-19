using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Menu.Commands;

/// <summary>
/// Command to update role permissions.
/// </summary>
public record UpdateRolePermissionsCommand : ICommand<Result>
{
    public Guid RoleId { get; init; }
    public UpdateRolePermissionsDto Permissions { get; init; } = new();
}

public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Role ID is required");
        RuleFor(x => x.Permissions).NotNull().WithMessage("Permissions are required");
    }
}

public class UpdateRolePermissionsCommandHandler : ICommandHandler<UpdateRolePermissionsCommand, Result>
{
    private readonly IMenuService _menuService;

    public UpdateRolePermissionsCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(UpdateRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.UpdateRolePermissionsAsync(command.RoleId, command.Permissions, cancellationToken);
    }
}
