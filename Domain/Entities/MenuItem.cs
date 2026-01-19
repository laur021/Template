namespace Domain.Entities;

/// <summary>
/// Represents a menu item (child level) within a section.
/// Example: "Phone", "Email", "User Management"
/// </summary>
public class MenuItem
{
    public Guid Id { get; set; }

    /// <summary>
    /// Parent section this menu item belongs to.
    /// </summary>
    public Guid SectionId { get; set; }

    /// <summary>
    /// Display name of the menu item.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Icon identifier.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Route path for navigation (e.g., "/records/phone").
    /// Null if this menu item only contains sub-items.
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// Order in which menu items appear within the section.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this menu item is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// If true, this menu item is visible to all authenticated users regardless of role.
    /// </summary>
    public bool IsVisibleToAll { get; set; } = false;

    /// <summary>
    /// When the menu item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the menu item was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
