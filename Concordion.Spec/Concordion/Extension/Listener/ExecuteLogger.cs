using Concordion.Api.Listener;

namespace Concordion.Spec.Concordion.Extension.Listener;

public class ExecuteLogger : IExecuteListener {
    private readonly TextWriter m_LogWriter;

    public ExecuteLogger(TextWriter logWriter)
    {
        m_LogWriter = logWriter;
    }

    public void ExecuteCompleted(ExecuteEvent executeEvent)
    {
        var element = executeEvent.Element;

        if (element.IsNamed("tr")) {
            var stringWriter = new StringWriter();

            stringWriter.Write("Execute '");

            var childElements = element.GetChildElements();
            var firstChild = true;

            foreach (var childElement in childElements) {
                if (firstChild)
                    firstChild = false;
                else
                    stringWriter.Write(", ");

                stringWriter.Write(childElement.Text);
            }

            stringWriter.Write("'");
            m_LogWriter.WriteLine(stringWriter.ToString());
        } else
            m_LogWriter.WriteLine("Execute '{0}'", element.Text);
    }
}
