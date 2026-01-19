namespace Application.Features.Menu.DTOs;

/// <summary>
/// Menu section in user's navigation.
/// </summary>
public record UserMenuSectionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public List<UserMenuItemDto> Items { get; init; } = new();
}
