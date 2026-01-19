namespace Domain.Entities;

/// <summary>
/// Defines which role can perform which page action.
/// This is the permission mapping between roles and page actions.
/// </summary>
public class RoleActionAccess
{
    public Guid Id { get; set; }

    /// <summary>
    /// The role this permission applies to (FK to AspNetRoles).
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// The action this permission applies to.
    /// </summary>
    public Guid ActionId { get; set; }

    /// <summary>
    /// Whether this role can perform this action.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// When this permission was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this permission was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
