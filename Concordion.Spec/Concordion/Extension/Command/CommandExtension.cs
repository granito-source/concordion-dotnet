using Concordion.Api.Extension;

namespace Concordion.Spec.Concordion.Extension.Command;

public class CommandExtension : IConcordionExtension {
    private readonly TextWriter m_LogWriter;

    public CommandExtension(TextWriter logWriter)
    {
        m_LogWriter = logWriter;
    }

    public void AddTo(IConcordionExtender concordionExtender)
    {
        concordionExtender.WithCommand("http://myorg.org/my/extension",
            "log", new LogCommand(m_LogWriter));
    }
}
