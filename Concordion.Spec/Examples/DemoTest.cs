using Concordion.NUnit;

namespace Concordion.Spec.Examples;

[ConcordionFixture]
public class DemoTest {
    public string greetingFor(string firstName)
    {
        return $"Hello {firstName}!";
    }
}
