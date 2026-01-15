using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertFalse;

[ConcordionFixture]
public class AssertFalseTest {
    public string successOrFailure(string fragment, string evaluationResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(bool.Parse(evaluationResult))
            .ProcessFragment(fragment)
            .SuccessOrFailureInWords();
    }
}
