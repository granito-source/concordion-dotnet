using System.Text.RegularExpressions;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.NonString;

[ConcordionFixture]
public class BooleanTest {
    public string OutcomeOfPerformingAssertEquals(string fragment,
        bool boolValue, string boolString)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(boolValue)
            .ProcessFragment(Regex.Replace(fragment,
                "\\(some boolean string\\)", boolString))
            .SuccessOrFailureInWords();
    }
}
