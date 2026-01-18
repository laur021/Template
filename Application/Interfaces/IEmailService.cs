namespace Application.Interfaces;

/// <summary>
/// Email service interface for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send an email confirmation link to the user.
    /// </summary>
    Task SendEmailConfirmationAsync(
        string toEmail,
        string confirmationLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a password reset link to the user.
    /// </summary>
    Task SendPasswordResetAsync(
        string toEmail,
        string resetLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a generic email.
    /// </summary>
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
