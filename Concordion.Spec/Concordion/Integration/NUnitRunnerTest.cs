using Concordion.Internal;
using Concordion.Spec.Support;
using NUnit.Framework;

namespace Concordion.Spec.Concordion.Integration;

[TestFixture]
public class NUnitRunnerTest {
    [Test]
    public void ConcordionTest()
    {
        var concordionResult = new FixtureRunner().Run(this);

        if (concordionResult.HasExceptions)
            throw new Exception(
                "Exception in Concordion test: please see Concordion test reports");

        if (concordionResult.HasFailures)
            Assert.Fail($"""
                Concordion Test Failures: {concordionResult.FailureCount}
                for stack trace, please see Concordion test reports
                """);
    }

    public bool GreetingsProcessed(string fragment)
    {
        return new TestRig()
            .WithFixture(this)
            .ProcessFragment(fragment)
            .IsSuccess;
    }

    public string GetGreeting()
    {
        return new Greeter().GetMessage();
    }
}

public class Greeter {
    public string GetMessage()
    {
        return "Hello World!";
    }
}
