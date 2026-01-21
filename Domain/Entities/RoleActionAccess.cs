namespace Domain.Entities;

public class RoleActionAccess
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid ActionId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
