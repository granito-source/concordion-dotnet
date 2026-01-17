using Concordion.Api;

namespace Concordion.Internal.Commands;

internal class ListExecuteStrategy : ExecuteStrategy {
    private const string LevelVariable = "#LEVEL";

    private static int GetLevel(Evaluator evaluator)
    {
        return evaluator.GetVariable(LevelVariable) is int intValue ?
            intValue : 0;
    }

    private static void IncreaseLevel(Evaluator evaluator)
    {
        if (evaluator.GetVariable(LevelVariable) == null)
            evaluator.SetVariable(LevelVariable, 1);
        else
            evaluator.SetVariable(LevelVariable, 1 + GetLevel(evaluator));
    }

    private static void DecreaseLevel(Evaluator evaluator)
    {
        evaluator.SetVariable(LevelVariable, GetLevel(evaluator) - 1);
    }

    public void Execute(CommandCall commandCall, Evaluator evaluator,
        ResultRecorder resultRecorder)
    {
        IncreaseLevel(evaluator);

        var listSupport = new ListSupport(commandCall);

        foreach (var listEntry in listSupport.GetListEntries()) {
            commandCall.Element = listEntry.Element;

            if (listEntry.IsItem)
                commandCall.Execute(evaluator, resultRecorder);

            if (listEntry.IsList)
                Execute(commandCall, evaluator, resultRecorder);
        }

        DecreaseLevel(evaluator);
    }
}
