using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals;

[ConcordionFixture]
public class CaseSensitiveTest {
    public string successOrFailure(string fragment, string evaluationResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(evaluationResult)
            .ProcessFragment(fragment)
            .SuccessOrFailureInWords();
    }
}
