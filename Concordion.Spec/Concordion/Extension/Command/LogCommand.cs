using Concordion.Api;
using Concordion.Internal;

namespace Concordion.Spec.Concordion.Extension.Command;

public class LogCommand : Api.Command {
    private TextWriter LogWriter { get; }

    public LogCommand(TextWriter logWriter)
    {
        LogWriter = logWriter;
    }

    public void Setup(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }

    public void Execute(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
        LogWriter.WriteLine(commandCall.Element.Text);
    }

    public void Verify(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
    }
}
