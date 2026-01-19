using Application.Core;
using Application.Features.Users.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Users.Queries;

/// <summary>
/// Query to get list of all users.
/// </summary>
public record GetUserListQuery : IQuery<Result<List<UserDto>>>;

public class GetUserListQueryHandler(
    IUserService userService) : IQueryHandler<GetUserListQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(
        GetUserListQuery query,
        CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync();
        return Result<List<UserDto>>.Success(users);
    }
}
