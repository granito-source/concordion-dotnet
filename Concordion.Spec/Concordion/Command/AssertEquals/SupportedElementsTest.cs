using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals;

[ConcordionFixture]
public class SupportedElementsTest {
    public string process(string snippet)
    {
        var successCount = new TestRig()
            .WithStubbedEvaluationResult("Fred")
            .ProcessFragment(snippet)
            .SuccessCount;

        return successCount == 1 ? snippet : "Did not work";
    }
}
