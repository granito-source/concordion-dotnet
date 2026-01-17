namespace Concordion.Api;

public class ResultDetails(Result result, string? message = null,
    string? stackTrace = null) {
    public string? Message { get; } = message;

    public string? StackTrace { get; } = stackTrace;

    public Exception? Exception { get; }

    public ResultDetails(Result result, Exception exception) : this(result)
    {
        Exception = exception;
    }

    public bool IsSuccess => result == Result.Success;

    public bool IsFailure => result == Result.Failure;

    public bool IsError => result == Result.Exception;
}
