namespace Concordion.Api.Listener;

public interface RunListener : ExceptionCaughtListener {
    void SuccessReported(RunSuccessEvent runSuccessEvent);

    void FailureReported(RunFailureEvent runFailureEvent);

    void IgnoredReported(RunIgnoreEvent runIgnoreEvent);
}
