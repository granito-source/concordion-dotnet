namespace Concordion.Internal;

internal interface FixtureState {
    void AssertIsSatisfied(long successCount, long failureCount, long exceptionCount);
}
