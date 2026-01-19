namespace Domain.Entities;

/// <summary>
/// Represents an action that can be performed on a page.
/// Example: "Create", "Edit", "Delete", "Export", "Generate Report"
/// </summary>
public class PageAction
{
    public Guid Id { get; set; }

    /// <summary>
    /// Menu item this action belongs to (if applicable).
    /// </summary>
    public Guid? MenuItemId { get; set; }

    /// <summary>
    /// Menu sub-item this action belongs to (if applicable).
    /// </summary>
    public Guid? MenuSubItemId { get; set; }

    /// <summary>
    /// Unique code for the action (e.g., "create", "edit", "delete", "export").
    /// Used for programmatic checks.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the action.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this action does.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Order in which actions appear.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this action is active/enabled globally.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the action was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the action was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
