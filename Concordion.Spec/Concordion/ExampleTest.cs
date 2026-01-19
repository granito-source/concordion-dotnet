using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion;

[ConcordionFixture]
public class ExampleTest {
    public string Greeting => "Hello World!";

    public string Process(string html)
    {
        return new TestRig()
            .WithFixture(this)
            .Process(html)
            .SuccessOrFailureInWords()
            .ToLower();
    }
}
