using Application.Contracts.Identity;
using Application.Core;
using Application.Interfaces;
using Cortex.Mediator.Queries;

namespace Application.Features.Auth.Queries;

/// <summary>
/// Query to get the current authenticated user's information.
/// </summary>
public record GetCurrentUserQuery : IQuery<Result<AuthUserDto>>;

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, Result<AuthUserDto>>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(IAuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task<Result<AuthUserDto>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
        {
            return Result<AuthUserDto>.Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(_currentUser.UserId.Value, cancellationToken);

        if (user == null)
        {
            return Result<AuthUserDto>.NotFound("User not found");
        }

        return Result<AuthUserDto>.Success(user);
    }
}
