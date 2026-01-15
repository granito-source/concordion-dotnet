using System.Globalization;
using System.Text.RegularExpressions;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.NonString;

[ConcordionFixture]
public class NonStringTest {
    public string outcomeOfPerformingAssertEquals(string fragment,
        string expectedString, string result, string resultType)
    {
        object simulatedResult;

        if (resultType.Equals("String")) {
            simulatedResult = result;
        } else if (resultType.Equals("Integer")) {
            simulatedResult = int.Parse(result);
        } else if (resultType.Equals("Double")) {
            var customCulture =
                (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();

            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;
            simulatedResult = double.Parse(result);
        } else {
            throw new Exception($"Unsupported result-type '{resultType}'.");
        }

        fragment = Regex.Replace(fragment, "\\(some expectation\\)",
            expectedString);

        return new TestRig()
            .WithStubbedEvaluationResult(simulatedResult)
            .ProcessFragment(fragment)
            .SuccessOrFailureInWords();
    }
}
