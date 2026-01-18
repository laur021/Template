using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Application role entity with Guid as the primary key.
/// </summary>
public class AppRole : IdentityRole<Guid>
{
    public AppRole() : base()
    {
    }

    public AppRole(string roleName) : base(roleName)
    {
    }

    /// <summary>
    /// Description of what this role allows.
    /// </summary>
    public string? Description { get; set; }
}
