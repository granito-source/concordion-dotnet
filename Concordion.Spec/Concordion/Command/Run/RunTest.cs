using Concordion.Api;
using Concordion.Integration;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Run;

[ConcordionTest]
public class RunTest {
    public string successOrFailure(string fragment,
        string hardCodedTestResult, string evaluationResult)
    {
        RunTestRunner.Result = (Result)Enum.Parse(typeof(Result),
            hardCodedTestResult, true);

        return new TestRig()
            .WithStubbedEvaluationResult(evaluationResult)
            .ProcessFragment(fragment)
            .SuccessOrFailureInWords();
    }
}
