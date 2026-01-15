using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Set;

[ConcordionFixture]
public class SetTest {
    private string? param;

    public void process(string fragment)
    {
        new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment);
    }

    public string? getParameterPassedIn()
    {
        return param;
    }

    public void setUpUser(string? fullName)
    {
        param = fullName;
    }
}
