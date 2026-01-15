using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Integration;

[ConcordionFixture]
public class ConstructorTest {
    public static int ConstructorCallCount { get; set; }

    public ConstructorTest()
    {
        ConstructorCallCount++;
    }

    public void ResetCounter()
    {
        ConstructorCallCount = 0;
    }
}
