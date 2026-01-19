namespace Application.Features.Menu.DTOs;

/// <summary>
/// Single action access update.
/// </summary>
public record ActionAccessUpdateDto
{
    /// <summary>
    /// Action ID.
    /// </summary>
    public Guid ActionId { get; init; }

    /// <summary>
    /// Whether the role can perform this action.
    /// </summary>
    public bool IsEnabled { get; init; }
}
