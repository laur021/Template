namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for creating a new menu sub-item.
/// </summary>
public record CreateMenuSubItemDto
{
    public Guid MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsVisibleToAll { get; init; } = false;
}
