namespace Application.Contracts.Identity;

/// <summary>
/// Result for email operations (confirmation, password reset).
/// </summary>
public record EmailResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; } = 200;

    public static EmailResult Success() => new() { Succeeded = true };
    public static EmailResult Failure(string error, int statusCode = 400) => new()
    {
        Succeeded = false,
        Error = error,
        StatusCode = statusCode
    };
}
