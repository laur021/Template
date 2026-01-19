namespace Application.Features.Users.DTOs;

public record UserDto(
    string Id,
    string Email,
    string UserName,
    string? FirstName,
    string? LastName,
    string? DisplayName,
    string? Bio,
    string? ImageUrl,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
