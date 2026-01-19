namespace Application.Features.Menu.DTOs;

/// <summary>
/// Action permission status.
/// </summary>
public record ActionPermissionDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}
