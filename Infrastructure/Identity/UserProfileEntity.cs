using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity;

/// <summary>
/// User profile/details entity - extends AspNetUsers with additional information.
/// This table has a 1:1 relationship with AspNetUsers (AppUser).
/// AspNetUsers handles authentication, this table stores extended profile details.
/// </summary>
[Table("UserProfiles")]
public class UserProfileEntity
{
    /// <summary>
    /// Primary key - same as AppUser.Id (foreign key to AspNetUsers).
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [MaxLength(100)]
    public string? LastName { get; set; }

    /// <summary>
    /// User's display name (nickname).
    /// </summary>
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Short biography or description.
    /// </summary>
    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// URL to user's profile image.
    /// </summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// User's phone number (additional to Identity's PhoneNumber).
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    [MaxLength(20)]
    public string? Gender { get; set; }

    /// <summary>
    /// User's address - street.
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }

    /// <summary>
    /// User's city.
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// User's state/province.
    /// </summary>
    [MaxLength(100)]
    public string? State { get; set; }

    /// <summary>
    /// User's country.
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// User's postal/zip code.
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the profile was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // ============ Navigation Properties ============

    /// <summary>
    /// Navigation property to the AppUser (AspNetUsers).
    /// </summary>
    [ForeignKey(nameof(Id))]
    public AppUser User { get; set; } = null!;
}
