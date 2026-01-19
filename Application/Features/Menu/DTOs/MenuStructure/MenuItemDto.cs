namespace Application.Features.Menu.DTOs;

/// <summary>
/// Full menu item details for admin.
/// </summary>
public record MenuItemDto
{
    public Guid Id { get; init; }
    public Guid SectionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<MenuSubItemDto> SubItems { get; init; } = new();
    public List<PageActionDto> Actions { get; init; } = new();
}
