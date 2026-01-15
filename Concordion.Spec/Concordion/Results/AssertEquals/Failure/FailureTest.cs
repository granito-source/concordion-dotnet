using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Results.AssertEquals.Failure;

[ConcordionFixture]
public class FailureTest {
    public string? acronym;

    public string renderAsFailure(string fragment, string acronym)
    {
        this.acronym = acronym;

        return new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment)
            .GetOutputFragmentXML()
            .Replace("\u00A0", "&#160;");
    }
}
