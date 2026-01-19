using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for PageAction with navigation properties.
/// </summary>
public class PageActionEntity
{
    public Guid Id { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? MenuSubItemId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public MenuItemEntity? MenuItem { get; set; }
    public MenuSubItemEntity? MenuSubItem { get; set; }
    public ICollection<RoleActionAccessEntity> RoleActionAccess { get; set; } = new List<RoleActionAccessEntity>();

    public PageAction ToDomain() => new()
    {
        Id = Id,
        MenuItemId = MenuItemId,
        MenuSubItemId = MenuSubItemId,
        Code = Code,
        Name = Name,
        Description = Description,
        DisplayOrder = DisplayOrder,
        IsActive = IsActive,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static PageActionEntity FromDomain(PageAction domain) => new()
    {
        Id = domain.Id,
        MenuItemId = domain.MenuItemId,
        MenuSubItemId = domain.MenuSubItemId,
        Code = domain.Code,
        Name = domain.Name,
        Description = domain.Description,
        DisplayOrder = domain.DisplayOrder,
        IsActive = domain.IsActive,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };
}
