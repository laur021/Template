namespace Application.Features.Menu.DTOs;

/// <summary>
/// Menu item in user's navigation.
/// </summary>
public record UserMenuItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Actions the user can perform on this page.
    /// </summary>
    public List<string> Actions { get; init; } = new();

    /// <summary>
    /// Sub-menu items if any.
    /// </summary>
    public List<UserMenuSubItemDto> SubItems { get; init; } = new();
}
