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
