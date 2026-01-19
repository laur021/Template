using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Application.Features.Menu.Commands;

#region Section Commands

/// <summary>
/// Command to create a menu section.
/// </summary>
public record CreateMenuSectionCommand : ICommand<Result<Guid>>
{
    public CreateMenuSectionDto Section { get; init; } = new();
}

public class CreateMenuSectionCommandValidator : AbstractValidator<CreateMenuSectionCommand>
{
    public CreateMenuSectionCommandValidator()
    {
        RuleFor(x => x.Section.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Section.Icon).MaximumLength(50);
    }
}

public class CreateMenuSectionCommandHandler : ICommandHandler<CreateMenuSectionCommand, Result<Guid>>
{
    private readonly IMenuService _menuService;

    public CreateMenuSectionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<Guid>> Handle(CreateMenuSectionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.CreateSectionAsync(command.Section, cancellationToken);
    }
}

/// <summary>
/// Command to update a menu section.
/// </summary>
public record UpdateMenuSectionCommand : ICommand<Result>
{
    public Guid Id { get; init; }
    public UpdateMenuSectionDto Section { get; init; } = new();
}

public class UpdateMenuSectionCommandValidator : AbstractValidator<UpdateMenuSectionCommand>
{
    public UpdateMenuSectionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Section.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Section.Icon).MaximumLength(50);
    }
}

public class UpdateMenuSectionCommandHandler : ICommandHandler<UpdateMenuSectionCommand, Result>
{
    private readonly IMenuService _menuService;

    public UpdateMenuSectionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(UpdateMenuSectionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.UpdateSectionAsync(command.Id, command.Section, cancellationToken);
    }
}

/// <summary>
/// Command to delete a menu section.
/// </summary>
public record DeleteMenuSectionCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}

public class DeleteMenuSectionCommandHandler : ICommandHandler<DeleteMenuSectionCommand, Result>
{
    private readonly IMenuService _menuService;

    public DeleteMenuSectionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(DeleteMenuSectionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.DeleteSectionAsync(command.Id, cancellationToken);
    }
}

#endregion

#region Menu Item Commands

/// <summary>
/// Command to create a menu item.
/// </summary>
public record CreateMenuItemCommand : ICommand<Result<Guid>>
{
    public CreateMenuItemDto MenuItem { get; init; } = new();
}

public class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.MenuItem.SectionId).NotEmpty();
        RuleFor(x => x.MenuItem.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MenuItem.Icon).MaximumLength(50);
        RuleFor(x => x.MenuItem.Route).MaximumLength(200);
    }
}

public class CreateMenuItemCommandHandler : ICommandHandler<CreateMenuItemCommand, Result<Guid>>
{
    private readonly IMenuService _menuService;

    public CreateMenuItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<Guid>> Handle(CreateMenuItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.CreateMenuItemAsync(command.MenuItem, cancellationToken);
    }
}

/// <summary>
/// Command to update a menu item.
/// </summary>
public record UpdateMenuItemCommand : ICommand<Result>
{
    public Guid Id { get; init; }
    public UpdateMenuItemDto MenuItem { get; init; } = new();
}

public class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MenuItem.SectionId).NotEmpty();
        RuleFor(x => x.MenuItem.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MenuItem.Icon).MaximumLength(50);
        RuleFor(x => x.MenuItem.Route).MaximumLength(200);
    }
}

public class UpdateMenuItemCommandHandler : ICommandHandler<UpdateMenuItemCommand, Result>
{
    private readonly IMenuService _menuService;

    public UpdateMenuItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(UpdateMenuItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.UpdateMenuItemAsync(command.Id, command.MenuItem, cancellationToken);
    }
}

/// <summary>
/// Command to delete a menu item.
/// </summary>
public record DeleteMenuItemCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}

public class DeleteMenuItemCommandHandler : ICommandHandler<DeleteMenuItemCommand, Result>
{
    private readonly IMenuService _menuService;

    public DeleteMenuItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(DeleteMenuItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.DeleteMenuItemAsync(command.Id, cancellationToken);
    }
}

#endregion

#region Sub-Item Commands

/// <summary>
/// Command to create a menu sub-item.
/// </summary>
public record CreateMenuSubItemCommand : ICommand<Result<Guid>>
{
    public CreateMenuSubItemDto SubItem { get; init; } = new();
}

public class CreateMenuSubItemCommandValidator : AbstractValidator<CreateMenuSubItemCommand>
{
    public CreateMenuSubItemCommandValidator()
    {
        RuleFor(x => x.SubItem.MenuItemId).NotEmpty();
        RuleFor(x => x.SubItem.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SubItem.Icon).MaximumLength(50);
        RuleFor(x => x.SubItem.Route).NotEmpty().MaximumLength(200);
    }
}

public class CreateMenuSubItemCommandHandler : ICommandHandler<CreateMenuSubItemCommand, Result<Guid>>
{
    private readonly IMenuService _menuService;

    public CreateMenuSubItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<Guid>> Handle(CreateMenuSubItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.CreateSubItemAsync(command.SubItem, cancellationToken);
    }
}

/// <summary>
/// Command to update a menu sub-item.
/// </summary>
public record UpdateMenuSubItemCommand : ICommand<Result>
{
    public Guid Id { get; init; }
    public UpdateMenuSubItemDto SubItem { get; init; } = new();
}

public class UpdateMenuSubItemCommandValidator : AbstractValidator<UpdateMenuSubItemCommand>
{
    public UpdateMenuSubItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SubItem.MenuItemId).NotEmpty();
        RuleFor(x => x.SubItem.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SubItem.Icon).MaximumLength(50);
        RuleFor(x => x.SubItem.Route).NotEmpty().MaximumLength(200);
    }
}

public class UpdateMenuSubItemCommandHandler : ICommandHandler<UpdateMenuSubItemCommand, Result>
{
    private readonly IMenuService _menuService;

    public UpdateMenuSubItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(UpdateMenuSubItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.UpdateSubItemAsync(command.Id, command.SubItem, cancellationToken);
    }
}

/// <summary>
/// Command to delete a menu sub-item.
/// </summary>
public record DeleteMenuSubItemCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}

public class DeleteMenuSubItemCommandHandler : ICommandHandler<DeleteMenuSubItemCommand, Result>
{
    private readonly IMenuService _menuService;

    public DeleteMenuSubItemCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(DeleteMenuSubItemCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.DeleteSubItemAsync(command.Id, cancellationToken);
    }
}

#endregion

#region Action Commands

/// <summary>
/// Command to create a page action.
/// </summary>
public record CreatePageActionCommand : ICommand<Result<Guid>>
{
    public CreatePageActionDto Action { get; init; } = new();
}

public class CreatePageActionCommandValidator : AbstractValidator<CreatePageActionCommand>
{
    public CreatePageActionCommandValidator()
    {
        RuleFor(x => x.Action.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Action.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Action.Description).MaximumLength(500);
    }
}

public class CreatePageActionCommandHandler : ICommandHandler<CreatePageActionCommand, Result<Guid>>
{
    private readonly IMenuService _menuService;

    public CreatePageActionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<Guid>> Handle(CreatePageActionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.CreateActionAsync(command.Action, cancellationToken);
    }
}

/// <summary>
/// Command to update a page action.
/// </summary>
public record UpdatePageActionCommand : ICommand<Result>
{
    public Guid Id { get; init; }
    public UpdatePageActionDto Action { get; init; } = new();
}

public class UpdatePageActionCommandValidator : AbstractValidator<UpdatePageActionCommand>
{
    public UpdatePageActionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Action.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Action.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Action.Description).MaximumLength(500);
    }
}

public class UpdatePageActionCommandHandler : ICommandHandler<UpdatePageActionCommand, Result>
{
    private readonly IMenuService _menuService;

    public UpdatePageActionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(UpdatePageActionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.UpdateActionAsync(command.Id, command.Action, cancellationToken);
    }
}

/// <summary>
/// Command to delete a page action.
/// </summary>
public record DeletePageActionCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}

public class DeletePageActionCommandHandler : ICommandHandler<DeletePageActionCommand, Result>
{
    private readonly IMenuService _menuService;

    public DeletePageActionCommandHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result> Handle(DeletePageActionCommand command, CancellationToken cancellationToken)
    {
        return await _menuService.DeleteActionAsync(command.Id, cancellationToken);
    }
}

#endregion
