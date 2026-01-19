using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Menu.Queries;

/// <summary>
/// Query to get the current user's menu based on their roles.
/// </summary>
public record GetUserMenuQuery : IQuery<Result<UserMenuDto>>;

public class GetUserMenuQueryHandler : IQueryHandler<GetUserMenuQuery, Result<UserMenuDto>>
{
    private readonly IMenuService _menuService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserMenuQueryHandler(IMenuService menuService, ICurrentUserService currentUserService)
    {
        _menuService = menuService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserMenuDto>> Handle(GetUserMenuQuery query, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
        {
            return Result<UserMenuDto>.Unauthorized();
        }

        var roles = _currentUserService.Roles ?? new List<string>();
        var menu = await _menuService.GetUserMenuAsync(roles, cancellationToken);

        return Result<UserMenuDto>.Success(menu);
    }
}
