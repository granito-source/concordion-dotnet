using Concordion.Api;

namespace Concordion.Internal;

internal class UnimplementedFixtureState : FixtureState {
    public void AssertIsSatisfied(long successCount, long failureCount,
        long exceptionCount)
    {
        if (successCount + failureCount + exceptionCount > 0)
            throw new AssertionErrorException(
                "Fixture is marked as Unimplemented but is reporting assertion(s).");
    }
}
