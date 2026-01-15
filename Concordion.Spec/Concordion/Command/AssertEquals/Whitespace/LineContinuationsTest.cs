using System.Text.RegularExpressions;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.Whitespace;

[ConcordionFixture]
public class LineContinuationsTest {
    private readonly List<string> snippets = [];

    public void addSnippet(string snippet)
    {
        snippets.Add(snippet);
    }

    public Result processSnippets(string evaluationResult)
    {
        var result = new Result();
        var i = 1;

        foreach (var snippet in snippets) {
            if (new TestRig()
                .WithStubbedEvaluationResult(evaluationResult)
                .ProcessFragment(snippet)
                .HasFailures)
                result.failures += "(" + i + "), ";
            else
                result.successes += "(" + i + "), ";

            i++;
        }

        result.failures = Regex.Replace(result.failures, ", $", "");
        result.successes = Regex.Replace(result.successes, ", $", "");

        return result;
    }

    public class Result {
        public string successes = "";

        public string failures = "";
    }
}
