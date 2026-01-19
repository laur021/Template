namespace Application.Features.Menu.DTOs;

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
