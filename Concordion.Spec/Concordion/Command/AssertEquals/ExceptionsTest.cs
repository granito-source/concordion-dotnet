using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals;

[ConcordionFixture]
public class ExceptionsTest {
    public object countsFromExecutingSnippetWithSimulatedEvaluationResult(
        string snippet, string simulatedResult)
    {
        var harness = new TestRig();

        if (simulatedResult.Equals("(An exception)"))
            harness.WithStubbedEvaluationResult(new Exception("simulated exception"));
        else
            harness.WithStubbedEvaluationResult(simulatedResult);

        return harness.ProcessFragment(snippet);
    }
}
