namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for updating a page action.
/// </summary>
public record UpdatePageActionDto
{
    public Guid? MenuItemId { get; init; }
    public Guid? MenuSubItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
