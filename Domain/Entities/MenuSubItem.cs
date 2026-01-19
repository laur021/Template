namespace Domain.Entities;

/// <summary>
/// Represents a menu sub-item (grandchild level) within a menu item.
/// Example: "Daily Report", "Monthly Report" under "Reports"
/// </summary>
public class MenuSubItem
{
    public Guid Id { get; set; }

    /// <summary>
    /// Parent menu item this sub-item belongs to.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Display name of the sub-item.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Icon identifier.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Route path for navigation (e.g., "/reports/daily").
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// Order in which sub-items appear within the menu item.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this sub-item is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// If true, this sub-item is visible to all authenticated users regardless of role.
    /// </summary>
    public bool IsVisibleToAll { get; set; } = false;

    /// <summary>
    /// When the sub-item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the sub-item was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
