using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertTrue;

[ConcordionFixture]
public class AssertTrueTest {
    public string successOrFailure(string fragment, string evaluationResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(bool.Parse(evaluationResult))
            .ProcessFragment(fragment)
            .SuccessOrFailureInWords();
    }
}
