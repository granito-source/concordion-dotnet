using Concordion.Api;

namespace Concordion.Spec.Concordion.Command.Run;

class RunTestRunner : IRunner {
    public static Result Result;

    public RunnerResult Execute(object fixture, Resource resource,
        string href)
    {
        return new RunnerResult(Result);
    }
}
