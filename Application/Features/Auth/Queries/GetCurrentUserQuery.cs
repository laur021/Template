using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Auth.Queries;

/// <summary>
/// Query to get the current authenticated user's information.
/// </summary>
public record GetCurrentUserQuery : IQuery<Result<UserDto>>;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(IAuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
        {
            return Result<UserDto>.Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(_currentUser.UserId.Value, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.NotFound("User not found");
        }

        return Result<UserDto>.Success(user);
    }
}
