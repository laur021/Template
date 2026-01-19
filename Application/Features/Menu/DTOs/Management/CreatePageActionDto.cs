namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for creating a new page action.
/// </summary>
public record CreatePageActionDto
{
    public Guid? MenuItemId { get; init; }
    public Guid? MenuSubItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
}
