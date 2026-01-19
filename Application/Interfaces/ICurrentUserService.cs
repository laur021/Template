namespace Application.Interfaces;

/// <summary>
/// Interface for accessing the current authenticated user.
/// Implemented in API layer using HttpContext.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The ID of the currently authenticated user, or null if not authenticated.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// The email of the currently authenticated user.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// The roles of the currently authenticated user.
    /// </summary>
    IEnumerable<string> Roles { get; }
}
