namespace Application.Features.Menu.DTOs;

/// <summary>
/// Full menu section details for admin.
/// </summary>
public record MenuSectionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<MenuItemDto> Items { get; init; } = new();
}
