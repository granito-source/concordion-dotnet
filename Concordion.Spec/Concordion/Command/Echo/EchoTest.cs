using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Echo;

[ConcordionFixture]
public class EchoTest {
    private string nextResult;

    public void setNextResult(string nextResult)
    {
        this.nextResult = nextResult;
    }

    public string render(string fragment)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(nextResult)
            .ProcessFragment(fragment)
            .GetOutputFragmentXML();
    }
}
