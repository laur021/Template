namespace Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Google authentication.
/// </summary>
public class GoogleAuthSettings
{
    public const string SectionName = "GoogleAuth";

    /// <summary>
    /// Google OAuth Client ID from Google Cloud Console.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}
