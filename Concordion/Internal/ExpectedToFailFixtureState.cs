using Concordion.Api;

namespace Concordion.Internal;

class ExpectedToFailFixtureState : FixtureState
{
    #region IFixtureState Members

    public void AssertIsSatisfied(long successCount, long failureCount, long exceptionCount)
    {
        if (failureCount + exceptionCount == 0)
        {
            throw new AssertionErrorException("Specification is expected to fail but has neither failures nor exceptions.");
        }
    }

    #endregion
}