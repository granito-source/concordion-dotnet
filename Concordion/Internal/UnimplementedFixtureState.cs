using Concordion.Api;

namespace Concordion.Internal;

class UnimplementedFixtureState : IFixtureState
{
    #region IFixtureState Members

    public void AssertIsSatisfied(long successCount, long failureCount, long exceptionCount)
    {
        if (successCount + failureCount + exceptionCount > 0)
        {
            throw new AssertionErrorException("Fixture is marked as Unimplemented but is reporting assertion(s).");
        }
    }

    #endregion
}