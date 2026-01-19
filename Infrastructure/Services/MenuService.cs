using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for menu and permission operations.
/// </summary>
public class MenuService : IMenuService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MenuService> _logger;

    public MenuService(AppDbContext context, ILogger<MenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region User Menu

    public async Task<UserMenuDto> GetUserMenuAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var roleNames = roles.ToList();

        // Get role IDs for the user's roles
        var roleIds = await _context.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        // Get all active sections with their items
        var sections = await _context.MenuSections
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .Include(s => s.MenuItems.Where(mi => mi.IsActive).OrderBy(mi => mi.DisplayOrder))
                .ThenInclude(mi => mi.SubItems.Where(si => si.IsActive).OrderBy(si => si.DisplayOrder))
            .Include(s => s.MenuItems)
                .ThenInclude(mi => mi.Actions.Where(a => a.IsActive))
            .Include(s => s.MenuItems)
                .ThenInclude(mi => mi.SubItems)
                    .ThenInclude(si => si.Actions.Where(a => a.IsActive))
            .ToListAsync(cancellationToken);

        // Get role menu access permissions
        var menuAccess = await _context.RoleMenuAccess
            .Where(rma => roleIds.Contains(rma.RoleId) && rma.HasAccess)
            .ToListAsync(cancellationToken);

        // Get role action access permissions
        var actionAccess = await _context.RoleActionAccess
            .Where(raa => roleIds.Contains(raa.RoleId) && raa.IsEnabled)
            .Select(raa => raa.ActionId)
            .ToListAsync(cancellationToken);

        var result = new UserMenuDto
        {
            Sections = new List<UserMenuSectionDto>()
        };

        foreach (var section in sections)
        {
            // Check if user has access to section
            var hasAccessToSection = section.IsVisibleToAll ||
                menuAccess.Any(ma => ma.SectionId == section.Id);

            if (!hasAccessToSection) continue;

            var sectionDto = new UserMenuSectionDto
            {
                Id = section.Id,
                Name = section.Name,
                Icon = section.Icon,
                DisplayOrder = section.DisplayOrder,
                Items = new List<UserMenuItemDto>()
            };

            foreach (var item in section.MenuItems)
            {
                // Check if user has access to menu item
                var hasAccessToItem = item.IsVisibleToAll ||
                    menuAccess.Any(ma => ma.MenuItemId == item.Id);

                if (!hasAccessToItem) continue;

                var itemDto = new UserMenuItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Icon = item.Icon,
                    Route = item.Route,
                    DisplayOrder = item.DisplayOrder,
                    Actions = item.Actions
                        .Where(a => actionAccess.Contains(a.Id))
                        .Select(a => a.Code)
                        .ToList(),
                    SubItems = new List<UserMenuSubItemDto>()
                };

                foreach (var subItem in item.SubItems)
                {
                    // Check if user has access to sub-item
                    var hasAccessToSubItem = subItem.IsVisibleToAll ||
                        menuAccess.Any(ma => ma.SubItemId == subItem.Id);

                    if (!hasAccessToSubItem) continue;

                    var subItemDto = new UserMenuSubItemDto
                    {
                        Id = subItem.Id,
                        Name = subItem.Name,
                        Icon = subItem.Icon,
                        Route = subItem.Route,
                        DisplayOrder = subItem.DisplayOrder,
                        Actions = subItem.Actions
                            .Where(a => actionAccess.Contains(a.Id))
                            .Select(a => a.Code)
                            .ToList()
                    };

                    itemDto.SubItems.Add(subItemDto);
                }

                sectionDto.Items.Add(itemDto);
            }

            // Only add section if it has accessible items
            if (sectionDto.Items.Count > 0)
            {
                result.Sections.Add(sectionDto);
            }
        }

        return result;
    }

    public async Task<bool> HasRouteAccessAsync(IEnumerable<string> roles, string route, CancellationToken cancellationToken = default)
    {
        var roleNames = roles.ToList();

        var roleIds = await _context.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        // Check menu items
        var menuItem = await _context.MenuItems
            .Where(mi => mi.Route == route && mi.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (menuItem != null)
        {
            if (menuItem.IsVisibleToAll) return true;

            return await _context.RoleMenuAccess
                .AnyAsync(rma => roleIds.Contains(rma.RoleId) &&
                                 rma.MenuItemId == menuItem.Id &&
                                 rma.HasAccess, cancellationToken);
        }

        // Check sub-items
        var subItem = await _context.MenuSubItems
            .Where(si => si.Route == route && si.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (subItem != null)
        {
            if (subItem.IsVisibleToAll) return true;

            return await _context.RoleMenuAccess
                .AnyAsync(rma => roleIds.Contains(rma.RoleId) &&
                                 rma.SubItemId == subItem.Id &&
                                 rma.HasAccess, cancellationToken);
        }

        return false;
    }

    public async Task<bool> CanPerformActionAsync(IEnumerable<string> roles, string route, string actionCode, CancellationToken cancellationToken = default)
    {
        var roleNames = roles.ToList();

        var roleIds = await _context.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        // Find the action by route and code
        var action = await _context.PageActions
            .Where(pa => pa.Code == actionCode && pa.IsActive)
            .Where(pa =>
                (pa.MenuItem != null && pa.MenuItem.Route == route) ||
                (pa.MenuSubItem != null && pa.MenuSubItem.Route == route))
            .FirstOrDefaultAsync(cancellationToken);

        if (action == null) return false;

        return await _context.RoleActionAccess
            .AnyAsync(raa => roleIds.Contains(raa.RoleId) &&
                             raa.ActionId == action.Id &&
                             raa.IsEnabled, cancellationToken);
    }

    #endregion

    #region Admin - Menu Structure Management

    public async Task<MenuStructureDto> GetMenuStructureAsync(CancellationToken cancellationToken = default)
    {
        var sections = await _context.MenuSections
            .OrderBy(s => s.DisplayOrder)
            .Include(s => s.MenuItems.OrderBy(mi => mi.DisplayOrder))
                .ThenInclude(mi => mi.SubItems.OrderBy(si => si.DisplayOrder))
                    .ThenInclude(si => si.Actions.OrderBy(a => a.DisplayOrder))
            .Include(s => s.MenuItems)
                .ThenInclude(mi => mi.Actions.OrderBy(a => a.DisplayOrder))
            .ToListAsync(cancellationToken);

        return new MenuStructureDto
        {
            Sections = sections.Select(s => new MenuSectionDto
            {
                Id = s.Id,
                Name = s.Name,
                Icon = s.Icon,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                IsVisibleToAll = s.IsVisibleToAll,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Items = s.MenuItems.Select(mi => new MenuItemDto
                {
                    Id = mi.Id,
                    SectionId = mi.SectionId,
                    Name = mi.Name,
                    Icon = mi.Icon,
                    Route = mi.Route,
                    DisplayOrder = mi.DisplayOrder,
                    IsActive = mi.IsActive,
                    IsVisibleToAll = mi.IsVisibleToAll,
                    CreatedAt = mi.CreatedAt,
                    UpdatedAt = mi.UpdatedAt,
                    Actions = mi.Actions.Select(a => new PageActionDto
                    {
                        Id = a.Id,
                        MenuItemId = a.MenuItemId,
                        MenuSubItemId = a.MenuSubItemId,
                        Code = a.Code,
                        Name = a.Name,
                        Description = a.Description,
                        DisplayOrder = a.DisplayOrder,
                        IsActive = a.IsActive
                    }).ToList(),
                    SubItems = mi.SubItems.Select(si => new MenuSubItemDto
                    {
                        Id = si.Id,
                        MenuItemId = si.MenuItemId,
                        Name = si.Name,
                        Icon = si.Icon,
                        Route = si.Route,
                        DisplayOrder = si.DisplayOrder,
                        IsActive = si.IsActive,
                        IsVisibleToAll = si.IsVisibleToAll,
                        CreatedAt = si.CreatedAt,
                        UpdatedAt = si.UpdatedAt,
                        Actions = si.Actions.Select(a => new PageActionDto
                        {
                            Id = a.Id,
                            MenuItemId = a.MenuItemId,
                            MenuSubItemId = a.MenuSubItemId,
                            Code = a.Code,
                            Name = a.Name,
                            Description = a.Description,
                            DisplayOrder = a.DisplayOrder,
                            IsActive = a.IsActive
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }

    public async Task<Result<Guid>> CreateSectionAsync(CreateMenuSectionDto dto, CancellationToken cancellationToken = default)
    {
        var section = new MenuSectionEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Icon = dto.Icon,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            IsVisibleToAll = dto.IsVisibleToAll,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuSections.Add(section);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created menu section {SectionName} with ID {SectionId}", dto.Name, section.Id);

        return Result<Guid>.Success(section.Id);
    }

    public async Task<Result> UpdateSectionAsync(Guid id, UpdateMenuSectionDto dto, CancellationToken cancellationToken = default)
    {
        var section = await _context.MenuSections.FindAsync(new object[] { id }, cancellationToken);

        if (section == null)
        {
            return Result.Failure("Section not found", 404);
        }

        section.Name = dto.Name;
        section.Icon = dto.Icon;
        section.DisplayOrder = dto.DisplayOrder;
        section.IsActive = dto.IsActive;
        section.IsVisibleToAll = dto.IsVisibleToAll;
        section.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated menu section {SectionId}", id);

        return Result.Success();
    }

    public async Task<Result> DeleteSectionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var section = await _context.MenuSections.FindAsync(new object[] { id }, cancellationToken);

        if (section == null)
        {
            return Result.Failure("Section not found", 404);
        }

        _context.MenuSections.Remove(section);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted menu section {SectionId}", id);

        return Result.Success();
    }

    public async Task<Result<Guid>> CreateMenuItemAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var sectionExists = await _context.MenuSections.AnyAsync(s => s.Id == dto.SectionId, cancellationToken);

        if (!sectionExists)
        {
            return Result<Guid>.Failure("Section not found", 404);
        }

        var item = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = dto.SectionId,
            Name = dto.Name,
            Icon = dto.Icon,
            Route = dto.Route,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            IsVisibleToAll = dto.IsVisibleToAll,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created menu item {ItemName} with ID {ItemId}", dto.Name, item.Id);

        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _context.MenuItems.FindAsync(new object[] { id }, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Menu item not found", 404);
        }

        item.SectionId = dto.SectionId;
        item.Name = dto.Name;
        item.Icon = dto.Icon;
        item.Route = dto.Route;
        item.DisplayOrder = dto.DisplayOrder;
        item.IsActive = dto.IsActive;
        item.IsVisibleToAll = dto.IsVisibleToAll;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated menu item {ItemId}", id);

        return Result.Success();
    }

    public async Task<Result> DeleteMenuItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.MenuItems.FindAsync(new object[] { id }, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Menu item not found", 404);
        }

        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted menu item {ItemId}", id);

        return Result.Success();
    }

    public async Task<Result<Guid>> CreateSubItemAsync(CreateMenuSubItemDto dto, CancellationToken cancellationToken = default)
    {
        var menuItemExists = await _context.MenuItems.AnyAsync(mi => mi.Id == dto.MenuItemId, cancellationToken);

        if (!menuItemExists)
        {
            return Result<Guid>.Failure("Menu item not found", 404);
        }

        var subItem = new MenuSubItemEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = dto.MenuItemId,
            Name = dto.Name,
            Icon = dto.Icon,
            Route = dto.Route,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            IsVisibleToAll = dto.IsVisibleToAll,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuSubItems.Add(subItem);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created menu sub-item {SubItemName} with ID {SubItemId}", dto.Name, subItem.Id);

        return Result<Guid>.Success(subItem.Id);
    }

    public async Task<Result> UpdateSubItemAsync(Guid id, UpdateMenuSubItemDto dto, CancellationToken cancellationToken = default)
    {
        var subItem = await _context.MenuSubItems.FindAsync(new object[] { id }, cancellationToken);

        if (subItem == null)
        {
            return Result.Failure("Sub-item not found", 404);
        }

        subItem.MenuItemId = dto.MenuItemId;
        subItem.Name = dto.Name;
        subItem.Icon = dto.Icon;
        subItem.Route = dto.Route;
        subItem.DisplayOrder = dto.DisplayOrder;
        subItem.IsActive = dto.IsActive;
        subItem.IsVisibleToAll = dto.IsVisibleToAll;
        subItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated menu sub-item {SubItemId}", id);

        return Result.Success();
    }

    public async Task<Result> DeleteSubItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subItem = await _context.MenuSubItems.FindAsync(new object[] { id }, cancellationToken);

        if (subItem == null)
        {
            return Result.Failure("Sub-item not found", 404);
        }

        _context.MenuSubItems.Remove(subItem);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted menu sub-item {SubItemId}", id);

        return Result.Success();
    }

    public async Task<Result<Guid>> CreateActionAsync(CreatePageActionDto dto, CancellationToken cancellationToken = default)
    {
        var action = new PageActionEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = dto.MenuItemId,
            MenuSubItemId = dto.MenuSubItemId,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.PageActions.Add(action);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created page action {ActionName} with ID {ActionId}", dto.Name, action.Id);

        return Result<Guid>.Success(action.Id);
    }

    public async Task<Result> UpdateActionAsync(Guid id, UpdatePageActionDto dto, CancellationToken cancellationToken = default)
    {
        var action = await _context.PageActions.FindAsync(new object[] { id }, cancellationToken);

        if (action == null)
        {
            return Result.Failure("Action not found", 404);
        }

        action.MenuItemId = dto.MenuItemId;
        action.MenuSubItemId = dto.MenuSubItemId;
        action.Code = dto.Code;
        action.Name = dto.Name;
        action.Description = dto.Description;
        action.DisplayOrder = dto.DisplayOrder;
        action.IsActive = dto.IsActive;
        action.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated page action {ActionId}", id);

        return Result.Success();
    }

    public async Task<Result> DeleteActionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var action = await _context.PageActions.FindAsync(new object[] { id }, cancellationToken);

        if (action == null)
        {
            return Result.Failure("Action not found", 404);
        }

        _context.PageActions.Remove(action);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted page action {ActionId}", id);

        return Result.Success();
    }

    #endregion

    #region Admin - Role Permissions

    public async Task<RolePermissionsDto?> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);

        if (role == null)
        {
            return null;
        }

        // Get all menu access for this role
        var menuAccess = await _context.RoleMenuAccess
            .Where(rma => rma.RoleId == roleId)
            .ToListAsync(cancellationToken);

        // Get all action access for this role
        var actionAccess = await _context.RoleActionAccess
            .Where(raa => raa.RoleId == roleId)
            .ToListAsync(cancellationToken);

        // Get all sections with items
        var sections = await _context.MenuSections
            .OrderBy(s => s.DisplayOrder)
            .Include(s => s.MenuItems.OrderBy(mi => mi.DisplayOrder))
                .ThenInclude(mi => mi.SubItems.OrderBy(si => si.DisplayOrder))
                    .ThenInclude(si => si.Actions.OrderBy(a => a.DisplayOrder))
            .Include(s => s.MenuItems)
                .ThenInclude(mi => mi.Actions.OrderBy(a => a.DisplayOrder))
            .ToListAsync(cancellationToken);

        return new RolePermissionsDto
        {
            RoleId = roleId,
            RoleName = role.Name!,
            Sections = sections.Select(s => new SectionPermissionDto
            {
                Id = s.Id,
                Name = s.Name,
                Icon = s.Icon,
                HasAccess = menuAccess.Any(ma => ma.SectionId == s.Id && ma.HasAccess),
                IsVisibleToAll = s.IsVisibleToAll,
                Items = s.MenuItems.Select(mi => new MenuItemPermissionDto
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    Icon = mi.Icon,
                    Route = mi.Route,
                    HasAccess = menuAccess.Any(ma => ma.MenuItemId == mi.Id && ma.HasAccess),
                    IsVisibleToAll = mi.IsVisibleToAll,
                    Actions = mi.Actions.Select(a => new ActionPermissionDto
                    {
                        Id = a.Id,
                        Code = a.Code,
                        Name = a.Name,
                        IsEnabled = actionAccess.Any(aa => aa.ActionId == a.Id && aa.IsEnabled)
                    }).ToList(),
                    SubItems = mi.SubItems.Select(si => new SubItemPermissionDto
                    {
                        Id = si.Id,
                        Name = si.Name,
                        Icon = si.Icon,
                        Route = si.Route,
                        HasAccess = menuAccess.Any(ma => ma.SubItemId == si.Id && ma.HasAccess),
                        IsVisibleToAll = si.IsVisibleToAll,
                        Actions = si.Actions.Select(a => new ActionPermissionDto
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Name = a.Name,
                            IsEnabled = actionAccess.Any(aa => aa.ActionId == a.Id && aa.IsEnabled)
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }

    public async Task<Result> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);

        if (role == null)
        {
            return Result.Failure("Role not found", 404);
        }

        // Update menu access
        foreach (var access in dto.MenuAccess)
        {
            RoleMenuAccessEntity? existing = null;

            if (access.SectionId.HasValue)
            {
                existing = await _context.RoleMenuAccess
                    .FirstOrDefaultAsync(rma => rma.RoleId == roleId && rma.SectionId == access.SectionId, cancellationToken);
            }
            else if (access.MenuItemId.HasValue)
            {
                existing = await _context.RoleMenuAccess
                    .FirstOrDefaultAsync(rma => rma.RoleId == roleId && rma.MenuItemId == access.MenuItemId, cancellationToken);
            }
            else if (access.SubItemId.HasValue)
            {
                existing = await _context.RoleMenuAccess
                    .FirstOrDefaultAsync(rma => rma.RoleId == roleId && rma.SubItemId == access.SubItemId, cancellationToken);
            }

            if (existing != null)
            {
                existing.HasAccess = access.HasAccess;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.RoleMenuAccess.Add(new RoleMenuAccessEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    SectionId = access.SectionId,
                    MenuItemId = access.MenuItemId,
                    SubItemId = access.SubItemId,
                    HasAccess = access.HasAccess,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // Update action access
        foreach (var access in dto.ActionAccess)
        {
            var existing = await _context.RoleActionAccess
                .FirstOrDefaultAsync(raa => raa.RoleId == roleId && raa.ActionId == access.ActionId, cancellationToken);

            if (existing != null)
            {
                existing.IsEnabled = access.IsEnabled;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.RoleActionAccess.Add(new RoleActionAccessEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    ActionId = access.ActionId,
                    IsEnabled = access.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated permissions for role {RoleId}", roleId);

        return Result.Success();
    }

    #endregion
}
