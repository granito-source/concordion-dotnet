namespace Concordion.Internal;

public class ChainOfExpectationCheckers : ExpectationChecker {
    private readonly List<ExpectationChecker> checkers = [];

    public ChainOfExpectationCheckers Add(ExpectationChecker checker)
    {
        checkers.Add(checker);

        return this;
    }

    public bool IsAcceptable(string expected, object? actual)
    {
        return checkers
            .Any(checker => checker.IsAcceptable(expected, actual));
    }
}
