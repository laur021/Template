using Application.Core;
using Application.Features.Menu.DTOs;

namespace Application.Interfaces;

/// <summary>
/// Service interface for menu and permission operations.
/// </summary>
public interface IMenuService
{
    #region User Menu

    /// <summary>
    /// Get the menu structure for a user based on their roles.
    /// Returns only sections/items/actions the user has access to.
    /// </summary>
    Task<UserMenuDto> GetUserMenuAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has access to a specific route.
    /// </summary>
    Task<bool> HasRouteAccessAsync(IEnumerable<string> roles, string route, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user can perform a specific action on a route.
    /// </summary>
    Task<bool> CanPerformActionAsync(IEnumerable<string> roles, string route, string actionCode, CancellationToken cancellationToken = default);

    #endregion

    #region Admin - Menu Structure Management

    /// <summary>
    /// Get the complete menu structure (for admin management).
    /// </summary>
    Task<MenuStructureDto> GetMenuStructureAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new menu section.
    /// </summary>
    Task<Result<Guid>> CreateSectionAsync(CreateMenuSectionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a menu section.
    /// </summary>
    Task<Result> UpdateSectionAsync(Guid id, UpdateMenuSectionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a menu section.
    /// </summary>
    Task<Result> DeleteSectionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new menu item.
    /// </summary>
    Task<Result<Guid>> CreateMenuItemAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a menu item.
    /// </summary>
    Task<Result> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a menu item.
    /// </summary>
    Task<Result> DeleteMenuItemAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new menu sub-item.
    /// </summary>
    Task<Result<Guid>> CreateSubItemAsync(CreateMenuSubItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a menu sub-item.
    /// </summary>
    Task<Result> UpdateSubItemAsync(Guid id, UpdateMenuSubItemDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a menu sub-item.
    /// </summary>
    Task<Result> DeleteSubItemAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new page action.
    /// </summary>
    Task<Result<Guid>> CreateActionAsync(CreatePageActionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a page action.
    /// </summary>
    Task<Result> UpdateActionAsync(Guid id, UpdatePageActionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a page action.
    /// </summary>
    Task<Result> DeleteActionAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Admin - Role Permissions

    /// <summary>
    /// Get all permissions for a specific role.
    /// </summary>
    Task<RolePermissionsDto?> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update permissions for a specific role.
    /// </summary>
    Task<Result> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsDto dto, CancellationToken cancellationToken = default);

    #endregion
}
