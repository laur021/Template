using Application.Core;
using Application.Features.Users.Commands;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return HandleResult(await Mediator.SendQueryAsync<GetUserListQuery, Result<List<UserDto>>>(new GetUserListQuery()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserDetail(string id)
    {
        return HandleResult(await Mediator.SendQueryAsync<GetUserDetailQuery, Result<UserDto>>(new GetUserDetailQuery { Id = id }));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<string>> CreateUser(CreateUserDto userDto)
    {
        return HandleResult(await Mediator.SendCommandAsync<CreateUserCommand, Result<string>>(new CreateUserCommand { UserDto = userDto }));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> EditUser(string id, EditUserDto userDto)
    {
        return HandleResult(await Mediator.SendCommandAsync<EditUserCommand, Result>(new EditUserCommand { UserDto = userDto with { Id = id } }));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        return HandleResult(await Mediator.SendCommandAsync<DeleteUserCommand, Result>(new DeleteUserCommand { Id = id }));
    }
}
