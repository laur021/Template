namespace Application.Features.Menu.DTOs;

/// <summary>
/// Page action details.
/// </summary>
public record PageActionDto
{
    public Guid Id { get; init; }
    public Guid? MenuItemId { get; init; }
    public Guid? MenuSubItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
