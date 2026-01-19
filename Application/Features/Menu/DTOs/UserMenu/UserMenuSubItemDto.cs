namespace Application.Features.Menu.DTOs;

/// <summary>
/// Sub-menu item in user's navigation.
/// </summary>
public record UserMenuSubItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Actions the user can perform on this page.
    /// </summary>
    public List<string> Actions { get; init; } = new();
}
