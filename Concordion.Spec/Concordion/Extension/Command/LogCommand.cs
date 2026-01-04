using Concordion.Api;
using Concordion.Internal;

namespace Concordion.Spec.Concordion.Extension.Command;

public class LogCommand : ICommand {
    private TextWriter LogWriter { get; }

    public LogCommand(TextWriter logWriter)
    {
        LogWriter = logWriter;
    }

    public void Setup(CommandCall commandCall, IEvaluator evaluator,
        IResultRecorder resultRecorder)
    {
    }

    public void Execute(CommandCall commandCall, IEvaluator evaluator,
        IResultRecorder resultRecorder)
    {
        LogWriter.WriteLine(commandCall.Element.Text);
    }

    public void Verify(CommandCall commandCall, IEvaluator evaluator,
        IResultRecorder resultRecorder)
    {
    }
}
