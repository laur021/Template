using Domain.Entities;

namespace Infrastructure.Identity;

/// <summary>
/// EF Core entity for RoleActionAccess with navigation properties.
/// </summary>
public class RoleActionAccessEntity
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid ActionId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public AppRole Role { get; set; } = null!;
    public PageActionEntity Action { get; set; } = null!;

    public RoleActionAccess ToDomain() => new()
    {
        Id = Id,
        RoleId = RoleId,
        ActionId = ActionId,
        IsEnabled = IsEnabled,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };

    public static RoleActionAccessEntity FromDomain(RoleActionAccess domain) => new()
    {
        Id = domain.Id,
        RoleId = domain.RoleId,
        ActionId = domain.ActionId,
        IsEnabled = domain.IsEnabled,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };
}
