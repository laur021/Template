namespace Application.Features.Menu.DTOs;

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
