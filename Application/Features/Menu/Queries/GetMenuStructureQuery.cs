using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Menu.Queries;

/// <summary>
/// Query to get the complete menu structure (admin only).
/// </summary>
public record GetMenuStructureQuery : IQuery<Result<MenuStructureDto>>;

public class GetMenuStructureQueryHandler : IQueryHandler<GetMenuStructureQuery, Result<MenuStructureDto>>
{
    private readonly IMenuService _menuService;

    public GetMenuStructureQueryHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<MenuStructureDto>> Handle(GetMenuStructureQuery query, CancellationToken cancellationToken)
    {
        var structure = await _menuService.GetMenuStructureAsync(cancellationToken);
        return Result<MenuStructureDto>.Success(structure);
    }
}
