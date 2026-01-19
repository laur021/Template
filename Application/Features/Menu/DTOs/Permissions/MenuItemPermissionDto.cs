namespace Application.Features.Menu.DTOs;

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
