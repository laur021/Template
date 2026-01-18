namespace Application.Core;

/// <summary>
/// Generic result wrapper for operation outcomes.
/// Enables consistent error handling without exceptions for expected failures.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public int StatusCode { get; private set; }
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }

    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value,
        StatusCode = 200
    };

    public static Result<T> Failure(string error, int statusCode = 400) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };

    public static Result<T> Unauthorized(string error = "Unauthorized") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 401
    };

    public static Result<T> Forbidden(string error = "Forbidden") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 403
    };

    public static Result<T> NotFound(string error = "Not found") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 404
    };

    public static Result<T> Conflict(string error) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 409
    };

    public static Result<T> ValidationFailure(IDictionary<string, string[]> errors) => new()
    {
        IsSuccess = false,
        Error = "One or more validation errors occurred",
        StatusCode = 400,
        ValidationErrors = errors
    };
}

/// <summary>
/// Non-generic result for operations that don't return a value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public int StatusCode { get; private set; }
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }

    public static Result Success() => new()
    {
        IsSuccess = true,
        StatusCode = 200
    };

    public static Result Failure(string error, int statusCode = 400) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };

    public static Result Unauthorized(string error = "Unauthorized") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 401
    };

    public static Result Forbidden(string error = "Forbidden") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 403
    };

    public static Result NotFound(string error = "Not found") => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 404
    };

    public static Result Conflict(string error) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = 409
    };

    public static Result ValidationFailure(IDictionary<string, string[]> errors) => new()
    {
        IsSuccess = false,
        Error = "One or more validation errors occurred",
        StatusCode = 400,
        ValidationErrors = errors
    };
}
