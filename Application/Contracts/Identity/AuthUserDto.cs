namespace Application.Contracts.Identity;

/// <summary>
/// DTO for authenticated user information returned after login/register.
/// </summary>
public record AuthUserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? ImageUrl { get; init; }
    public bool EmailConfirmed { get; init; }
}
