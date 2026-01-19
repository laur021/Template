using Application.Core;
using Application.Features.Users.DTOs;
using Application.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for user management operations.
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<AppUser> userManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Include(u => u.Profile)
            .ToListAsync();
        return users.Select(MapToUserDto).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<Result<string>> CreateUserAsync(
        string email,
        string userName,
        string? displayName,
        string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Result<string>.Failure("A user with this email already exists", 409);
        }

        var existingUserName = await _userManager.FindByNameAsync(userName);
        if (existingUserName != null)
        {
            return Result<string>.Failure("A user with this username already exists", 409);
        }

        var user = new AppUser
        {
            UserName = userName,
            Email = email,
            DisplayName = displayName ?? userName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User creation failed for {Email}: {Errors}", email, errors);
            return Result<string>.Failure(errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        _logger.LogInformation("User {Email} created successfully with ID {UserId}", email, user.Id);

        return Result<string>.Success(user.Id.ToString());
    }

    public async Task<Result> UpdateUserAsync(
        string userId,
        string email,
        string? displayName,
        string? bio)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found", 404);
        }

        // Check if email is being changed and if it's already taken
        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result.Failure("A user with this email already exists", 409);
            }
            user.Email = email;
            user.UserName = email;
        }

        user.DisplayName = displayName;
        user.Bio = bio;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User update failed for {UserId}: {Errors}", userId, errors);
            return Result.Failure(errors);
        }

        _logger.LogInformation("User {UserId} updated successfully", userId);

        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found", 404);
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User deletion failed for {UserId}: {Errors}", userId, errors);
            return Result.Failure(errors);
        }

        _logger.LogInformation("User {UserId} deleted successfully", userId);

        return Result.Success();
    }

    private static UserDto MapToUserDto(AppUser user)
    {
        return new UserDto(
            Id: user.Id.ToString(),
            Email: user.Email!,
            UserName: user.UserName!,
            FirstName: user.Profile?.FirstName,
            LastName: user.Profile?.LastName,
            DisplayName: user.Profile?.DisplayName ?? user.DisplayName,
            Bio: user.Profile?.Bio ?? user.Bio,
            ImageUrl: user.Profile?.ImageUrl ?? user.ImageUrl,
            PhoneNumber: user.Profile?.PhoneNumber,
            DateOfBirth: user.Profile?.DateOfBirth,
            Gender: user.Profile?.Gender,
            Address: user.Profile?.Address,
            City: user.Profile?.City,
            State: user.Profile?.State,
            Country: user.Profile?.Country,
            PostalCode: user.Profile?.PostalCode,
            CreatedAt: user.Profile?.CreatedAt ?? user.CreatedAt,
            UpdatedAt: user.Profile?.UpdatedAt);
    }
}
