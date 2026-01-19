namespace Application.Features.Users.DTOs;

public record UserDto(
    string Id,
    string Email,
    string UserName,
    string? DisplayName,
    string? Bio,
    string? Image,
    DateTime CreatedAt);
