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
