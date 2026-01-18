namespace Application.Contracts.Identity;

/// <summary>
/// Result for operations that affect refresh tokens (logout, revoke).
/// </summary>
public record TokenResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; } = 200;

    public static TokenResult Success() => new() { Succeeded = true };
    public static TokenResult Failure(string error, int statusCode = 400) => new()
    {
        Succeeded = false,
        Error = error,
        StatusCode = statusCode
    };
}
