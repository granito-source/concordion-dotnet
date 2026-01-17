using Concordion.Api;
using Concordion.NUnit;
using Concordion.Spec.Support;
using NUnit.Framework;

namespace Concordion.Spec.Concordion.Command.Run;

// [Ignore("address run command failures")]
// [ConcordionFixture]
public class RunTest {
    public string SuccessOrFailure(string fragment,
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
