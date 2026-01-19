namespace Domain.Entities;

/// <summary>
/// User profile/details entity - extends AspNetUsers with additional information.
/// This table has a 1:1 relationship with AspNetUsers (AppUser).
/// AspNetUsers handles authentication, this table stores extended profile details.
/// </summary>
public class UserProfile
{
    /// <summary>
    /// Primary key - same as AppUser.Id (foreign key to AspNetUsers).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// User's display name (nickname).
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Short biography or description.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// URL to user's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// User's phone number (additional to Identity's PhoneNumber).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// User's address - street.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// User's city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// User's state/province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// User's country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// User's postal/zip code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the profile was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
