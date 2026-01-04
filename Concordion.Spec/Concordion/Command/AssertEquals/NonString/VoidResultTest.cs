using Concordion.Integration;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.NonString;

[ConcordionTest]
public class VoidResultTest {
    public string process(string snippet)
    {
        var r = new TestRig()
            .WithFixture(this)
            .ProcessFragment(snippet);

        return r.ExceptionCount != 0 ? "exception" :
            r.SuccessOrFailureInWords();
    }

    public void myVoidMethod()
    {
    }
}
