namespace Gateway.Common;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public class Result<T>
{
    private Result(T? value, string? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public bool IsSuccess => string.IsNullOrEmpty(Error);
    public string? Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string error) => new(default, error);

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result
{
    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string? Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}