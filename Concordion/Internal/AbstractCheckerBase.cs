using System.Text.RegularExpressions;

namespace Concordion.Internal;

public abstract partial class AbstractCheckerBase : ExpectationChecker {
    [GeneratedRegex(@"[\s]+")]
    private static partial Regex SpaceRegex();

    [GeneratedRegex(@" _")]
    private static partial Regex LineContinuation();

    [GeneratedRegex(@"\r?\n")]
    private static partial Regex NewLineRegex();

    public abstract bool IsAcceptable(string expected, object? actual);

    public string Normalize(object? obj)
    {
        var s = ConvertObjectToString(obj);

        s = ProcessLineContinuations(s);
        s = StripNewlines(s);
        s = ReplaceMultipleWhitespaceWithOneSpace(s);

        return s.Trim();
    }

    private string ReplaceMultipleWhitespaceWithOneSpace(string s)
    {
        return SpaceRegex().Replace(s, " ");
    }

    private string ProcessLineContinuations(string s)
    {
        return LineContinuation().Replace(s, string.Empty);
    }

    private string StripNewlines(string s)
    {
        return NewLineRegex().Replace(s, string.Empty);
    }

    private string ConvertObjectToString(object? obj)
    {
        return obj?.ToString() ?? "(null)";
    }
}
