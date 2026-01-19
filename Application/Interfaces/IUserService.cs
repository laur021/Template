using Application.Core;
using Application.Features.Users.DTOs;

namespace Application.Interfaces;

/// <summary>
/// Service interface for user management operations.
/// </summary>
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<Result<string>> CreateUserAsync(string email, string userName, string? displayName, string password);
    Task<Result> UpdateUserAsync(string userId, string email, string? displayName, string? bio);
    Task<Result> DeleteUserAsync(string userId);
}
