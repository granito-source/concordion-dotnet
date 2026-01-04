namespace Concordion.Api.Listener;

public interface IAssertListener
{
    void SuccessReported(AssertSuccessEvent successEvent);

    void FailureReported(AssertFailureEvent failureEvent);
}
