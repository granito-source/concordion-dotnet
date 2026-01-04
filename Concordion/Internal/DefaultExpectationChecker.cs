namespace Concordion.Internal;

public class DefaultExpectationChecker : AbstractCheckerBase
{
    public override bool IsAcceptable(string expected, object actual)
    {
        return Normalize(expected).Equals(Normalize(actual));
    }
}