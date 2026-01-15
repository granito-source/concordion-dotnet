using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.Execute;

[ConcordionFixture]
public class AccessToLinkHrefTest {
    public bool fragmentSucceeds(string fragment)
    {
        var result = new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment);

        return result.IsSuccess && result.SuccessCount > 0;
    }

    public string myMethod(string s)
    {
        return s;
    }
}
