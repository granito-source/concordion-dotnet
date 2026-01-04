namespace Concordion.Internal;

public interface IExpectationChecker
{
    bool IsAcceptable(string expected, object actual);
}