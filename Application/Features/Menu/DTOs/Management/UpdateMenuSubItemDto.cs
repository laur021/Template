namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for updating a menu sub-item.
/// </summary>
public record UpdateMenuSubItemDto
{
    public Guid MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}
