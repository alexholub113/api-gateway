namespace Gateway.Common.Models.Result;

/// <summary>
/// Objects from Error class cause a failed result
/// </summary>
public class Error
{
    private static readonly Error _emptyError = new(string.Empty);

    /// <summary>
    /// Creates a new instance of <see cref="Error"/>
    /// </summary>
    /// <param name="message">Description of the error</param>
    private Error(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Error"/>
    /// </summary>
    /// <param name="message">Description of the error</param>
    /// <param name="exception">Exception that caused the error</param>
    private Error(string message, Exception exception) : this(message)
    {
        Exception = exception;
    }

    /// <summary>
    /// Message of the error
    /// </summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Exception that caused the error
    /// </summary>
    public Exception? Exception { get; protected set; }

    public static Error None => _emptyError;

    public static Error Create(string errorMessage)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(errorMessage, nameof(errorMessage));
        return new Error(errorMessage);
    }

    public static Error Create(string errorMessage, Exception exception)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(errorMessage, nameof(errorMessage));
        return new Error(errorMessage, exception);
    }

    public static Error NotFound(string message)
    {
        return new Error(message);
    }
}
