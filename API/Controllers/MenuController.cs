using Application.Core;
using Application.Features.Menu.DTOs;
using Application.Features.Menu.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Menu endpoints for user navigation.
/// Returns menus and actions based on user's role permissions.
/// </summary>
public class MenuController : BaseApiController
{
    /// <summary>
    /// Gets the current user's menu based on their roles.
    /// Returns only sections, menus, and actions the user has access to.
    /// </summary>
    /// <returns>User's accessible menu structure.</returns>
    /// <response code="200">Returns the user's menu.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserMenuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserMenuDto>> GetUserMenu()
    {
        return HandleResult(await Mediator.SendQueryAsync<GetUserMenuQuery, Result<UserMenuDto>>(new GetUserMenuQuery()));
    }
}
