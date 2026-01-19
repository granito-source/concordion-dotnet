using Concordion.Internal;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Configuration;

[ConcordionFixture]
public class BaseInputDirectoryTest {
    private static bool inTestRun;

    public bool DirectoryBasedExecuted(string baseInputDirectory)
    {
        if (inTestRun)
            return true;

        inTestRun = true;

        var specificationConfig = new SpecificationConfig().Load(GetType());

        specificationConfig.BaseInputDirectory = baseInputDirectory;

        var testResult = new FixtureRunner(specificationConfig).Run(this);

        inTestRun = false;

        foreach (var failureDetail in testResult.FailureDetails) {
            Console.WriteLine(failureDetail.Message);
            Console.WriteLine(failureDetail.StackTrace);
        }

        foreach (var errorDetail in testResult.ErrorDetails) {
            Console.WriteLine(errorDetail.Message);
            Console.WriteLine(errorDetail.StackTrace);
            Console.WriteLine(errorDetail.Exception);
        }

        return !testResult.HasFailures && !testResult.HasExceptions;
    }

    public bool EmbeddedExecuted()
    {
        if (inTestRun)
            return true;

        inTestRun = true;

        var specificationConfig = new SpecificationConfig().Load(GetType());

        specificationConfig.BaseInputDirectory = null;

        var fixtureRunner = new FixtureRunner(specificationConfig);
        var testResult = fixtureRunner.Run(this);

        inTestRun = false;

        return !testResult.HasFailures && !testResult.HasExceptions;
    }
}
