using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Menu.Queries;

/// <summary>
/// Query to get permissions for a specific role (admin only).
/// </summary>
public record GetRolePermissionsQuery : IQuery<Result<RolePermissionsDto>>
{
    public Guid RoleId { get; init; }
}

public class GetRolePermissionsQueryHandler : IQueryHandler<GetRolePermissionsQuery, Result<RolePermissionsDto>>
{
    private readonly IMenuService _menuService;

    public GetRolePermissionsQueryHandler(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<Result<RolePermissionsDto>> Handle(GetRolePermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await _menuService.GetRolePermissionsAsync(query.RoleId, cancellationToken);

        if (permissions == null)
        {
            return Result<RolePermissionsDto>.NotFound("Role not found");
        }

        return Result<RolePermissionsDto>.Success(permissions);
    }
}
