namespace Concordion.Internal;

interface IFixtureState
{
    void AssertIsSatisfied(long successCount, long failureCount, long exceptionCount);
}