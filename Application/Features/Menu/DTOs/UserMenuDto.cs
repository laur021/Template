namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for user's menu - returned by GET /api/menu
/// Contains only the menus and actions the user has access to.
/// </summary>
public record UserMenuDto
{
    /// <summary>
    /// List of menu sections the user has access to.
    /// </summary>
    public List<UserMenuSectionDto> Sections { get; init; } = new();
}

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
