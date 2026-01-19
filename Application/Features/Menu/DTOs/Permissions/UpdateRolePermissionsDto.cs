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
