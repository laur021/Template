namespace Domain.Entities;

/// <summary>
/// Represents a menu section (parent level) in the navigation.
/// Example: "RECORDS", "REPORTS", "SETTINGS"
/// </summary>
public class MenuSection
{
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the section.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Icon identifier (e.g., "folder", "settings", "chart").
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Order in which sections appear in navigation.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this section is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// If true, this section is visible to all authenticated users regardless of role.
    /// </summary>
    public bool IsVisibleToAll { get; set; } = false;

    /// <summary>
    /// When the section was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the section was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
