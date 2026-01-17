using Concordion.Internal;

namespace Concordion.Api;

public abstract class AbstractCommand : Command {
    public virtual void Setup(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
    }

    public virtual void Execute(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
    }

    public virtual void Verify(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
    }
}
