using Concordion.Api.Listener;

namespace Concordion.Spec.Concordion.Extension.Listener;

public class AssertLogger : IAssertEqualsListener, IAssertTrueListener,
    IAssertFalseListener {
    private readonly TextWriter m_LogWriter;

    public AssertLogger(TextWriter logWriter)
    {
        m_LogWriter = logWriter;
    }

    public void SuccessReported(AssertSuccessEvent successEvent)
    {
        m_LogWriter.WriteLine("Success '{0}'", successEvent.Element.Text);
    }

    public void FailureReported(AssertFailureEvent failureEvent)
    {
        m_LogWriter.WriteLine("Failure expected:'{0}' actual:'{1}'",
            failureEvent.Expected, failureEvent.Actual);
    }
}
