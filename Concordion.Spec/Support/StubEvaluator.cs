using Concordion.Api;

namespace Concordion.Spec.Support;

public class StubEvaluator(object fixture) : Evaluator, EvaluatorFactory {
    private object? evaluationResult;

    public Evaluator CreateEvaluator(object fixture)
    {
        return this;
    }

    public object? Evaluate(string expression)
    {
        if (evaluationResult is Exception exception)
            throw exception;

        return evaluationResult;
    }

    public object? GetVariable(string variableName)
    {
        return null;
    }

    public void SetVariable(string variableName, object? value)
    {
    }

    public object Fixture { get; } = fixture;

    public EvaluatorFactory WithStubbedResult(object? result)
    {
        evaluationResult = result;

        return this;
    }
}
