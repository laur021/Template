namespace Application.Contracts.Identity;

/// <summary>
/// Information about an external login provider (Google, etc.)
/// </summary>
public record ExternalAuthInfo
{
    public string Provider { get; init; } = string.Empty;
    public string ProviderKey { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? ImageUrl { get; init; }
}
