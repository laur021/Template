namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for updating role permissions - PUT /api/admin/roles/{roleId}/permissions
/// </summary>
public record UpdateRolePermissionsDto
{
    /// <summary>
    /// Menu access updates (sections, items, sub-items).
    /// </summary>
    public List<MenuAccessUpdateDto> MenuAccess { get; init; } = new();

    /// <summary>
    /// Action access updates.
    /// </summary>
    public List<ActionAccessUpdateDto> ActionAccess { get; init; } = new();
}

/// <summary>
/// Single menu access update.
/// </summary>
public record MenuAccessUpdateDto
{
    /// <summary>
    /// Section ID (if updating section access).
    /// </summary>
    public Guid? SectionId { get; init; }

    /// <summary>
    /// Menu item ID (if updating menu access).
    /// </summary>
    public Guid? MenuItemId { get; init; }

    /// <summary>
    /// Sub-item ID (if updating sub-menu access).
    /// </summary>
    public Guid? SubItemId { get; init; }

    /// <summary>
    /// Whether the role has access.
    /// </summary>
    public bool HasAccess { get; init; }
}

/// <summary>
/// Single action access update.
/// </summary>
public record ActionAccessUpdateDto
{
    /// <summary>
    /// Action ID.
    /// </summary>
    public Guid ActionId { get; init; }

    /// <summary>
    /// Whether the role can perform this action.
    /// </summary>
    public bool IsEnabled { get; init; }
}
