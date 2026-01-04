using System.Text.RegularExpressions;

namespace Concordion.Internal;

public class BooleanExpectationChecker : AbstractCheckerBase
{

    public override bool IsAcceptable(string expected, object actual)
    {
        if (!(actual is bool)) return false;

        var boolActual = (bool) actual;
        var normalizedExpected = Normalize(expected).ToLower();
        return (boolActual && Regex.IsMatch(normalizedExpected, "^(true|yes|y)$")) ||
               (!boolActual && Regex.IsMatch(normalizedExpected, "^(false|no|n|-)$"));
    }
}