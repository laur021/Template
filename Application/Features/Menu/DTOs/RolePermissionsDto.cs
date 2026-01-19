namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for role permissions - returned by GET /api/admin/roles/{roleId}/permissions
/// </summary>
public record RolePermissionsDto
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;

    /// <summary>
    /// All sections with their access status for this role.
    /// </summary>
    public List<SectionPermissionDto> Sections { get; init; } = new();
}

/// <summary>
/// Section permission status.
/// </summary>
public record SectionPermissionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public bool HasAccess { get; init; }
    public bool IsVisibleToAll { get; init; }
    public List<MenuItemPermissionDto> Items { get; init; } = new();
}

/// <summary>
/// Menu item permission status.
/// </summary>
public record MenuItemPermissionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public bool HasAccess { get; init; }
    public bool IsVisibleToAll { get; init; }
    public List<SubItemPermissionDto> SubItems { get; init; } = new();
    public List<ActionPermissionDto> Actions { get; init; } = new();
}

/// <summary>
/// Sub-item permission status.
/// </summary>
public record SubItemPermissionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public bool HasAccess { get; init; }
    public bool IsVisibleToAll { get; init; }
    public List<ActionPermissionDto> Actions { get; init; } = new();
}

/// <summary>
/// Action permission status.
/// </summary>
public record ActionPermissionDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}
