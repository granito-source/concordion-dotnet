using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals;

[ConcordionFixture]
public class NestedHtmlElementsTest {
    public string matchOrNotMatch(string snippet, string evaluationResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(evaluationResult)
            .ProcessFragment(snippet)
            .HasFailures ? "not match" : "match";
    }
}
