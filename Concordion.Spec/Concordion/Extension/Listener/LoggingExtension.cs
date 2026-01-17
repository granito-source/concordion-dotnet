using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Listener;

public class LoggingExtension(TextWriter logWriter) : ConcordionExtension {
    private readonly AssertLogger m_AssertLogger = new(logWriter);

    private readonly ExecuteLogger m_ExecuteLogger = new(logWriter);

    private readonly VerifyRowsLogger m_VerifyRowsLogger = new(logWriter);

    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithAssertEqualsListener(m_AssertLogger);
        concordionExtender.WithAssertTrueListener(m_AssertLogger);
        concordionExtender.WithAssertFalseListener(m_AssertLogger);
        concordionExtender.WithExecuteListener(m_ExecuteLogger);
        concordionExtender.WithVerifyRowsListener(m_VerifyRowsLogger);
    }
}
