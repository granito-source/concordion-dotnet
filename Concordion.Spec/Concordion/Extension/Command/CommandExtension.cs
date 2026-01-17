using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Command;

public class CommandExtension : ConcordionExtension {
    private readonly TextWriter m_LogWriter;

    public CommandExtension(TextWriter logWriter)
    {
        m_LogWriter = logWriter;
    }

    public void AddTo(ConcordionExtender concordionExtender)
    {
        concordionExtender.WithCommand("http://myorg.org/my/extension",
            "log", new LogCommand(m_LogWriter));
    }
}
