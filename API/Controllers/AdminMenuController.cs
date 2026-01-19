using Application.Core;
using Application.Features.Menu.Commands;
using Application.Features.Menu.DTOs;
using Application.Features.Menu.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Admin endpoints for managing menu structure and role permissions.
/// </summary>
[Authorize(Policy = "Admin")]
[Route("api/admin/[controller]")]
public class AdminMenuController : BaseApiController
{
    #region Menu Structure Management

    /// <summary>
    /// Gets the complete menu structure for admin management.
    /// </summary>
    /// <returns>Complete menu structure with all sections, items, and actions.</returns>
    [HttpGet("structure")]
    [ProducesResponseType(typeof(MenuStructureDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MenuStructureDto>> GetMenuStructure()
    {
        return HandleResult(await Mediator.SendQueryAsync<GetMenuStructureQuery, Result<MenuStructureDto>>(new GetMenuStructureQuery()));
    }

    #region Sections

    /// <summary>
    /// Creates a new menu section.
    /// </summary>
    [HttpPost("sections")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateSection([FromBody] CreateMenuSectionDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreateMenuSectionCommand, Result<Guid>>(
            new CreateMenuSectionCommand { Section = dto }));
    }

    /// <summary>
    /// Updates an existing menu section.
    /// </summary>
    [HttpPut("sections/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateSection(Guid id, [FromBody] UpdateMenuSectionDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<UpdateMenuSectionCommand, Result>(
            new UpdateMenuSectionCommand { Id = id, Section = dto }));
    }

    /// <summary>
    /// Deletes a menu section and all its items.
    /// </summary>
    [HttpDelete("sections/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSection(Guid id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeleteMenuSectionCommand, Result>(
            new DeleteMenuSectionCommand { Id = id }));
    }

    #endregion

    #region Menu Items

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateMenuItem([FromBody] CreateMenuItemDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreateMenuItemCommand, Result<Guid>>(
            new CreateMenuItemCommand { MenuItem = dto }));
    }

    /// <summary>
    /// Updates an existing menu item.
    /// </summary>
    [HttpPut("items/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<UpdateMenuItemCommand, Result>(
            new UpdateMenuItemCommand { Id = id, MenuItem = dto }));
    }

    /// <summary>
    /// Deletes a menu item and all its sub-items.
    /// </summary>
    [HttpDelete("items/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMenuItem(Guid id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeleteMenuItemCommand, Result>(
            new DeleteMenuItemCommand { Id = id }));
    }

    #endregion

    #region Sub-Items

    /// <summary>
    /// Creates a new menu sub-item.
    /// </summary>
    [HttpPost("subitems")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateSubItem([FromBody] CreateMenuSubItemDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreateMenuSubItemCommand, Result<Guid>>(
            new CreateMenuSubItemCommand { SubItem = dto }));
    }

    /// <summary>
    /// Updates an existing menu sub-item.
    /// </summary>
    [HttpPut("subitems/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateSubItem(Guid id, [FromBody] UpdateMenuSubItemDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<UpdateMenuSubItemCommand, Result>(
            new UpdateMenuSubItemCommand { Id = id, SubItem = dto }));
    }

    /// <summary>
    /// Deletes a menu sub-item.
    /// </summary>
    [HttpDelete("subitems/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSubItem(Guid id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeleteMenuSubItemCommand, Result>(
            new DeleteMenuSubItemCommand { Id = id }));
    }

    #endregion

    #region Actions

    /// <summary>
    /// Creates a new page action.
    /// </summary>
    [HttpPost("actions")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateAction([FromBody] CreatePageActionDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreatePageActionCommand, Result<Guid>>(
            new CreatePageActionCommand { Action = dto }));
    }

    /// <summary>
    /// Updates an existing page action.
    /// </summary>
    [HttpPut("actions/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAction(Guid id, [FromBody] UpdatePageActionDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<UpdatePageActionCommand, Result>(
            new UpdatePageActionCommand { Id = id, Action = dto }));
    }

    /// <summary>
    /// Deletes a page action.
    /// </summary>
    [HttpDelete("actions/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAction(Guid id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeletePageActionCommand, Result>(
            new DeletePageActionCommand { Id = id }));
    }

    #endregion

    #endregion

    #region Role Permissions Management

    /// <summary>
    /// Gets all permissions for a specific role.
    /// Returns all sections, menus, and actions with their current access status.
    /// </summary>
    [HttpGet("roles/{roleId}/permissions")]
    [ProducesResponseType(typeof(RolePermissionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RolePermissionsDto>> GetRolePermissions(Guid roleId)
    {
        return HandleResult(await Mediator.SendQueryAsync<GetRolePermissionsQuery, Result<RolePermissionsDto>>(
            new GetRolePermissionsQuery { RoleId = roleId }));
    }

    /// <summary>
    /// Updates permissions for a specific role.
    /// Use this to grant/revoke menu access and enable/disable actions.
    /// </summary>
    [HttpPut("roles/{roleId}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateRolePermissions(Guid roleId, [FromBody] UpdateRolePermissionsDto dto)
    {
        return HandleResult(await Mediator.SendCommandAsync<UpdateRolePermissionsCommand, Result>(
            new UpdateRolePermissionsCommand { RoleId = roleId, Permissions = dto }));
    }

    #endregion
}
