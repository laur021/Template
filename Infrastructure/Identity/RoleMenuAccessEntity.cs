using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for RoleMenuAccess with navigation properties.
/// </summary>
public class RoleMenuAccessEntity
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? SubItemId { get; set; }
    public bool HasAccess { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public AppRole Role { get; set; } = null!;
    public MenuSectionEntity? Section { get; set; }
    public MenuItemEntity? MenuItem { get; set; }
    public MenuSubItemEntity? SubItem { get; set; }

    public RoleMenuAccess ToDomain() => new()
    {
        Id = Id,
        RoleId = RoleId,
        SectionId = SectionId,
        MenuItemId = MenuItemId,
        SubItemId = SubItemId,
        HasAccess = HasAccess,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static RoleMenuAccessEntity FromDomain(RoleMenuAccess domain) => new()
    {
        Id = domain.Id,
        RoleId = domain.RoleId,
        SectionId = domain.SectionId,
        MenuItemId = domain.MenuItemId,
        SubItemId = domain.SubItemId,
        HasAccess = domain.HasAccess,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };
}
