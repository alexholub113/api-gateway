namespace Gateway.Common.Models.Result;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public class Result<T>
{
    private Result(T value, Error error)
    {
        Value = value;
        Error = error;
    }

    public T Value { get; }
    public bool IsSuccess => !IsFailure;
    public Error Error { get; }
    public bool IsFailure => Error != Error.None;

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return new(value, Error.None);
    }

    public static Result<T> Failure(string errorMessage) => Failure(Error.Create(errorMessage));
    public static Result<T> Failure(Error error) => new(default!, error);

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result
{
    private Result(Error error)
    {
        Error = error;
    }

    public bool IsSuccess => !IsFailure;
    public Error Error { get; }
    public bool IsFailure => Error != Error.None;

    public static Result Success() => new(Error.None);
    public static Result Failure(string errorMessage) => Failure(Error.Create(errorMessage));
    public static Result Failure(Error error) => new(error);
}