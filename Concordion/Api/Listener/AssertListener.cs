namespace Concordion.Api.Listener;

public interface AssertListener {
    void SuccessReported(AssertSuccessEvent successEvent);

    void FailureReported(AssertFailureEvent failureEvent);
}
