using Concordion.Integration;

namespace Concordion.Spec.Examples;

[ConcordionTest]
public class DemoTest {
    public string greetingFor(string firstName)
    {
        return $"Hello {firstName}!";
    }
}
