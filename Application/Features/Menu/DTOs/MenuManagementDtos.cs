namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for creating a new menu section.
/// </summary>
public record CreateMenuSectionDto
{
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsVisibleToAll { get; init; } = false;
}

/// <summary>
/// DTO for updating a menu section.
/// </summary>
public record UpdateMenuSectionDto
{
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}

/// <summary>
/// DTO for creating a new menu item.
/// </summary>
public record CreateMenuItemDto
{
    public Guid SectionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsVisibleToAll { get; init; } = false;
}

/// <summary>
/// DTO for updating a menu item.
/// </summary>
public record UpdateMenuItemDto
{
    public Guid SectionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}

/// <summary>
/// DTO for creating a new menu sub-item.
/// </summary>
public record CreateMenuSubItemDto
{
    public Guid MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsVisibleToAll { get; init; } = false;
}

/// <summary>
/// DTO for updating a menu sub-item.
/// </summary>
public record UpdateMenuSubItemDto
{
    public Guid MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string Route { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisibleToAll { get; init; }
}

/// <summary>
/// DTO for creating a new page action.
/// </summary>
public record CreatePageActionDto
{
    public Guid? MenuItemId { get; init; }
    public Guid? MenuSubItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
}

/// <summary>
/// DTO for updating a page action.
/// </summary>
public record UpdatePageActionDto
{
    public Guid? MenuItemId { get; init; }
    public Guid? MenuSubItemId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
