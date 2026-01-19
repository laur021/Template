namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for updating a menu section.
/// </summary>
public record UpdateMenuSectionDto
{
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}
