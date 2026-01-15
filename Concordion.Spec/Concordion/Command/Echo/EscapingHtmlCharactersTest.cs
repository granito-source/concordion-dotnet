using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Echo;

[ConcordionFixture]
public class EscapingHtmlCharactersTest {
    public string render(string fragment, string evalResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(evalResult)
            .ProcessFragment(fragment)
            .GetOutputFragmentXML()
            .Replace(" concordion:echo=\"username\">", ">");
    }
}
