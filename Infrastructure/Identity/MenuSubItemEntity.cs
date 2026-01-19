using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for MenuSubItem with navigation properties.
/// </summary>
public class MenuSubItemEntity
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Route { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisibleToAll { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public MenuItemEntity MenuItem { get; set; } = null!;
    public ICollection<PageActionEntity> Actions { get; set; } = new List<PageActionEntity>();
    public ICollection<RoleMenuAccessEntity> RoleMenuAccess { get; set; } = new List<RoleMenuAccessEntity>();

    public MenuSubItem ToDomain() => new()
    {
        Id = Id,
        MenuItemId = MenuItemId,
        Name = Name,
        Icon = Icon,
        Route = Route,
        DisplayOrder = DisplayOrder,
        IsActive = IsActive,
        IsVisibleToAll = IsVisibleToAll,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static MenuSubItemEntity FromDomain(MenuSubItem domain) => new()
    {
        Id = domain.Id,
        MenuItemId = domain.MenuItemId,
        Name = domain.Name,
        Icon = domain.Icon,
        Route = domain.Route,
        DisplayOrder = domain.DisplayOrder,
        IsActive = domain.IsActive,
        IsVisibleToAll = domain.IsVisibleToAll,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };
}
