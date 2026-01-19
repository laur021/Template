namespace Application.Features.Users.DTOs;

public record CreateUserDto(
    string Email,
    string UserName,
    string? DisplayName,
    string Password);
