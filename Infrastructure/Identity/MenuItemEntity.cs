using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for MenuItem with navigation properties.
/// </summary>
public class MenuItemEntity
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisibleToAll { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public MenuSectionEntity Section { get; set; } = null!;
    public ICollection<MenuSubItemEntity> SubItems { get; set; } = new List<MenuSubItemEntity>();
    public ICollection<PageActionEntity> Actions { get; set; } = new List<PageActionEntity>();
    public ICollection<RoleMenuAccessEntity> RoleMenuAccess { get; set; } = new List<RoleMenuAccessEntity>();

    public MenuItem ToDomain() => new()
    {
        Id = Id,
        SectionId = SectionId,
        Name = Name,
        Icon = Icon,
        Route = Route,
        DisplayOrder = DisplayOrder,
        IsActive = IsActive,
        IsVisibleToAll = IsVisibleToAll,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static MenuItemEntity FromDomain(MenuItem domain) => new()
    {
        Id = domain.Id,
        SectionId = domain.SectionId,
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
