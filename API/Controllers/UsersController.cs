using Application.Core;
using Application.Features.Users.Commands;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// User management endpoints.
/// Provides CRUD operations for user accounts.
/// </summary>
public class UsersController : BaseApiController
{
    /// <summary>
    /// Gets a list of all users.
    /// </summary>
    /// <returns>List of user DTOs.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return HandleResult(await Mediator.SendQueryAsync<GetUserListQuery, Result<List<UserDto>>>(new GetUserListQuery()));
    }

    /// <summary>
    /// Gets detailed information about a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>User details.</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserDetail(string id)
    {
        return HandleResult(await Mediator.SendQueryAsync<GetUserDetailQuery, Result<UserDto>>(new GetUserDetailQuery { Id = id }));
    }

    /// <summary>
    /// Creates a new user account (Admin only).
    /// </summary>
    /// <param name="userDto">User creation data.</param>
    /// <returns>The ID of the newly created user.</returns>
    /// <response code="200">User created successfully, returns user ID.</response>
    /// <response code="400">Invalid user data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have admin privileges.</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<string>> CreateUser(CreateUserDto userDto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreateUserCommand, Result<string>>(new CreateUserCommand { UserDto = userDto }));
    }

    /// <summary>
    /// Updates an existing user account (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userDto">Updated user data.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Invalid user data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have admin privileges.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EditUser(string id, EditUserDto userDto)
    {
        return HandleResult(await Mediator.SendCommandAsync<EditUserCommand, Result>(new EditUserCommand { UserDto = userDto with { Id = id } }));
    }

    /// <summary>
    /// Deletes a user account (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>Success result.</returns>
    /// <response code="200">User deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have admin privileges.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(string id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeleteUserCommand, Result>(new DeleteUserCommand { Id = id }));
    }
}
