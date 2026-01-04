using Concordion.Api;

namespace Concordion.Internal.Commands;

class ListExecuteStrategy : IExecuteStrategy
{
    private static readonly string LEVEL_VARIABLE = "#LEVEL";

    public void Execute(CommandCall commandCall, IEvaluator evaluator, IResultRecorder resultRecorder)
    {
        increaseLevel(evaluator);

        var listSupport = new ListSupport(commandCall);

        foreach (var listEntry in listSupport.GetListEntries())
        {
            commandCall.Element = listEntry.Element;
            if (listEntry.IsItem)
            {
                commandCall.Execute(evaluator, resultRecorder);
            }
            if (listEntry.IsList)
            {
                Execute(commandCall, evaluator, resultRecorder);
            }
        }
        decreaseLevel(evaluator);
    }

    private void increaseLevel(IEvaluator evaluator)
    {
        if (evaluator.GetVariable(LEVEL_VARIABLE) == null)
        {
            evaluator.SetVariable(LEVEL_VARIABLE, 1);
        }
        else
        {
            evaluator.SetVariable(LEVEL_VARIABLE, 1 + (int)evaluator.GetVariable(LEVEL_VARIABLE));
        }
    }

    private void decreaseLevel(IEvaluator evaluator)
    {
        evaluator.SetVariable(LEVEL_VARIABLE, (int)evaluator.GetVariable(LEVEL_VARIABLE) - 1);
    }
}
