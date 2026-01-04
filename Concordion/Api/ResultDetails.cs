namespace Concordion.Api;

/// <summary>
///
/// </summary>
public class ResultDetails
{
    public Result Result { get; private set; }

    public string Message { get; private set; }

    public string StackTrace { get; private set; }

    public Exception Exception { get; private set; }

    public ResultDetails(Result result, string message, string stackTrace)
    {
        Result = result;
        Message = message;
        StackTrace = stackTrace;
    }

    public ResultDetails(Result result, Exception exception)
    {
        Result = result;
        Exception = exception;
    }

    public ResultDetails(Result result)
    {
        Result = result;
    }

    public bool IsSuccess
    {
        get { return Result == Result.Success; }
    }

    public bool IsFailure
    {
        get { return Result == Result.Failure; }
    }

    public bool IsError
    {
        get { return Result == Result.Exception; }
    }
}
