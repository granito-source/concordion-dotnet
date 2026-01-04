using Concordion.Internal;
using NUnit.Framework;

namespace Concordion.Runners.NUnit;

[TestFixture]
public class ExecutableSpecification {
    [Test]
    public void ConcordionTest()
    {
        var concordionResult = new FixtureRunner().Run(this);

        if (concordionResult.HasExceptions)
            throw new Exception("Exception in Concordion test: please see Concordion test reports");

        if (concordionResult.HasFailures)
            Assert.Fail("Concordion Test Failures: " +
                concordionResult.FailureCount,
                "for stack trace, please see Concordion test reports");
        else
            Assert.Pass();
    }
}
