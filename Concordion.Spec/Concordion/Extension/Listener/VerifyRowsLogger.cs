using Concordion.Api.Listener;
using Concordion.Internal;

namespace Concordion.Spec.Concordion.Extension.Listener;

public class VerifyRowsLogger : VerifyRowsListener {
    private readonly TextWriter m_LogWriter;

    public VerifyRowsLogger(TextWriter logWriter)
    {
        m_LogWriter = logWriter;
    }

    public void ExpressionEvaluated(
        ExpressionEvaluatedEvent expressionEvaluatedEvent)
    {
        m_LogWriter.WriteLine("Evaluated '{0}'",
            expressionEvaluatedEvent
                .Element
                .GetAttributeValue("verifyRows",
                    ConcordionBuilder.ConcordionNamespace));
    }

    public void MissingRow(MissingRowEvent missingRowEvent)
    {
        m_LogWriter.WriteLine("Missing Row '{0}'",
            missingRowEvent.RowElement.Text);
    }

    public void SurplusRow(SurplusRowEvent surplusRowEvent)
    {
        m_LogWriter.WriteLine("Surplus Row '{0}'",
            surplusRowEvent.RowElement.Text);
    }
}
