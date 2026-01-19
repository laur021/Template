namespace Application.Features.Menu.DTOs;

/// <summary>
/// DTO for admin menu management - full menu structure.
/// </summary>
public record MenuStructureDto
{
    public List<MenuSectionDto> Sections { get; init; } = new();
}
