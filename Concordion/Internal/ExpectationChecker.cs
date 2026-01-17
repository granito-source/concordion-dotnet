namespace Concordion.Internal;

public interface ExpectationChecker {
    bool IsAcceptable(string expected, object? actual);
}
