namespace Domain.Entities;

/// <summary>
/// Defines which role has access to which menu section/item/sub-item.
/// This is the permission mapping between roles and menu navigation.
/// </summary>
public class RoleMenuAccess
{
    public Guid Id { get; set; }

    /// <summary>
    /// The role this permission applies to (FK to AspNetRoles).
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Section access (if granting section-level access).
    /// </summary>
    public Guid? SectionId { get; set; }

    /// <summary>
    /// Menu item access (if granting menu-level access).
    /// </summary>
    public Guid? MenuItemId { get; set; }

    /// <summary>
    /// Sub-item access (if granting sub-menu level access).
    /// </summary>
    public Guid? SubItemId { get; set; }

    /// <summary>
    /// Whether this role has access to the specified menu element.
    /// </summary>
    public bool HasAccess { get; set; } = true;

    /// <summary>
    /// When this permission was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this permission was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
