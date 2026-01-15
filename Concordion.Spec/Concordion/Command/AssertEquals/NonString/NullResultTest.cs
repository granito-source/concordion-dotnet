using System.Text.RegularExpressions;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.NonString;

[ConcordionFixture]
public class NullResultTest {
    public string outcomeOfPerformingAssertEquals(string snippet,
        string expectedString, string result)
    {
        if (result.Equals("null"))
            result = null;

        snippet = Regex.Replace(snippet, "\\(some expectation\\)",
            expectedString);

        return new TestRig()
            .WithStubbedEvaluationResult(result)
            .ProcessFragment(snippet)
            .SuccessOrFailureInWords();
    }
}
