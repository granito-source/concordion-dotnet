using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Echo;

[ConcordionFixture]
public class DisplayingNullsTest {
    public string render(string fragment)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(null)
            .ProcessFragment(fragment)
            .GetOutputFragmentXML()
            .Replace(" concordion:echo=\"username\">", ">");
    }
}
