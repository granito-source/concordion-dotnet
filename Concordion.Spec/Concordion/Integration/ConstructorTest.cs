using Concordion.Integration;

namespace Concordion.Spec.Concordion.Integration;

[ConcordionTest]
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
