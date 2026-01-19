using Application.Core;
using Application.Features.Users.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Users.Queries;

/// <summary>
/// Query to get user details by ID.
/// </summary>
public record GetUserDetailQuery : IQuery<Result<UserDto>>
{
    public string Id { get; init; } = string.Empty;
}

public class GetUserDetailQueryHandler(
    IUserService userService) : IQueryHandler<GetUserDetailQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetUserDetailQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(query.Id);

        if (user == null)
            return Result<UserDto>.Failure("User not found", 404);

        return Result<UserDto>.Success(user);
    }
}
