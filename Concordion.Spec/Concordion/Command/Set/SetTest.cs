using Concordion.Integration;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Set;

[ConcordionTest]
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

    public void setUpUser(string fullName)
    {
        param = fullName;
    }
}
