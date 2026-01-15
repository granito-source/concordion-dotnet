using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion;

[ConcordionFixture]
public class ExampleTest {
    public string greeting = "Hello World!";

    public string process(string html)
    {
        return new TestRig()
            .WithFixture(this)
            .Process(html)
            .SuccessOrFailureInWords()
            .ToLower();
    }
}
