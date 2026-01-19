namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for updating a menu item.
/// </summary>
public record UpdateMenuItemDto
{
    public Guid SectionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}
