using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for MenuSection with navigation properties.
/// </summary>
public class MenuSectionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisibleToAll { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<MenuItemEntity> MenuItems { get; set; } = new List<MenuItemEntity>();
    public ICollection<RoleMenuAccessEntity> RoleMenuAccess { get; set; } = new List<RoleMenuAccessEntity>();

    public MenuSection ToDomain() => new()
    {
        Id = Id,
        Name = Name,
        Icon = Icon,
        DisplayOrder = DisplayOrder,
        IsActive = IsActive,
        IsVisibleToAll = IsVisibleToAll,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static MenuSectionEntity FromDomain(MenuSection domain) => new()
    {
        Id = domain.Id,
        Name = domain.Name,
        Icon = domain.Icon,
        DisplayOrder = domain.DisplayOrder,
        IsActive = domain.IsActive,
        IsVisibleToAll = domain.IsVisibleToAll,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };
}
