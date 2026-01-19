namespace Application.Features.Users.DTOs;

public record EditUserDto(
    string Id,
    string Email,
    string? DisplayName,
    string Bio);
