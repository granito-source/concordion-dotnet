using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Results.AssertEquals.Success;

[ConcordionFixture]
public class EmptySuccessTest : SuccessTest {
    public EmptySuccessTest()
    {
        username = string.Empty;
    }
}
