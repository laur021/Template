namespace Application.Features.Menu.DTOs;

/// <summary>
/// Single menu access update.
/// </summary>
public record MenuAccessUpdateDto
{
    /// <summary>
    /// Section ID (if updating section access).
    /// </summary>
    public Guid? SectionId { get; init; }

    /// <summary>
    /// Menu item ID (if updating menu access).
    /// </summary>
    public Guid? MenuItemId { get; init; }

    /// <summary>
    /// Sub-item ID (if updating sub-menu access).
    /// </summary>
    public Guid? SubItemId { get; init; }

    /// <summary>
    /// Whether the role has access.
    /// </summary>
    public bool HasAccess { get; init; }
}
