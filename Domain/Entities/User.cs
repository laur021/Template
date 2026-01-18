namespace Domain.Entities;

/// <summary>
/// Domain entity representing a user in the system.
/// This is a POCO with NO dependencies on ASP.NET Identity or any framework.
/// The actual Identity user (AppUser) in Infrastructure inherits from IdentityUser<Guid>
/// and maps to this domain entity when needed.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
