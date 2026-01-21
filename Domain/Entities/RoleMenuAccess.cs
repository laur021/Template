namespace Domain.Entities;

public class RoleMenuAccess
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? SubItemId { get; set; }
    public bool HasAccess { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
