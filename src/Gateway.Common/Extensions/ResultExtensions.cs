namespace Gateway.Common.Extensions;

/// <summary>
/// Extension methods for Result types to enable fluent chaining
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result to a new result type
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsFailure
            ? Result<TOut>.Failure(result.Error)
            : Result<TOut>.Success(mapper(result.Value));
    }

    /// <summary>
    /// Chains a successful result to another result-returning operation
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> binder)
    {
        return result.IsFailure
            ? Result<TOut>.Failure(result.Error)
            : binder(result.Value);
    }

    /// <summary>
    /// Chains a successful result to an async result-returning operation
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> binder)
    {
        return result.IsFailure
            ? Result<TOut>.Failure(result.Error)
            : await binder(result.Value);
    }

    /// <summary>
    /// Chains a successful result to a non-generic Result operation
    /// </summary>
    public static async Task<Result> BindAsync<TIn>(this Result<TIn> result, Func<TIn, Task<Result>> binder)
    {
        return result.IsFailure
            ? Result.Failure(result.Error)
            : await binder(result.Value);
    }

    /// <summary>
    /// Chains a Task of Result to another async result-returning operation
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        return result.IsFailure
            ? Result<TOut>.Failure(result.Error)
            : await binder(result.Value);
    }

    /// <summary>
    /// Chains a Task of Result to a sync result-returning operation
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Result<TOut>> binder)
    {
        var result = await resultTask;
        return result.IsFailure
            ? Result<TOut>.Failure(result.Error)
            : binder(result.Value);
    }

    /// <summary>
    /// Chains a Task of Result to a non-generic async Result operation
    /// </summary>
    public static async Task<Result> BindAsync<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> binder)
    {
        var result = await resultTask;
        return result.IsFailure
            ? Result.Failure(result.Error)
            : await binder(result.Value);
    }

    /// <summary>
    /// Chains two successful results together
    /// </summary>
    public static Result<(TFirst First, TSecond Second)> Combine<TFirst, TSecond>(
        this Result<TFirst> first,
        Result<TSecond> second)
    {
        if (first.IsFailure)
            return Result<(TFirst, TSecond)>.Failure(first.Error);

        if (second.IsFailure)
            return Result<(TFirst, TSecond)>.Failure(second.Error);

        return Result<(TFirst, TSecond)>.Success((first.Value, second.Value));
    }
}