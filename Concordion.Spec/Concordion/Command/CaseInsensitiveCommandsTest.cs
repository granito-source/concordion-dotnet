using Concordion.Integration;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command;

[ConcordionTest]
public class CaseInsensitiveCommands {
    public string process(string snippet, object stubbedResult)
    {
        var successCount = new TestRig()
            .WithStubbedEvaluationResult(stubbedResult)
            .ProcessFragment(snippet)
            .SuccessCount;

        return successCount == 1 ? snippet : "Did not work";
    }
}
