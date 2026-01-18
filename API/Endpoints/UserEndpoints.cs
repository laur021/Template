using Application.Contracts.Identity;
using Application.Core;
using Application.Features.Auth.Queries;
using Cortex.Mediator;

namespace API.Endpoints;

/// <summary>
/// User-related endpoints using Minimal API pattern.
/// </summary>
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get the current authenticated user's information")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetCurrentUser(IMediator mediator)
    {
        var result = await mediator.SendQueryAsync<GetCurrentUserQuery, Result<UserDto>>(
            new GetCurrentUserQuery());

        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: result.StatusCode == 401 ? "Unauthorized" : "Error",
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Results.Ok(result.Value);
    }
}
