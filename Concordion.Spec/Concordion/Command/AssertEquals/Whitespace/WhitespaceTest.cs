using System.Text.RegularExpressions;
using Concordion.Internal;
using Concordion.NUnit;
using Concordion.Spec.Support;

namespace Concordion.Spec.Concordion.Command.AssertEquals.Whitespace;

[ConcordionFixture]
public class WhitespaceTest {
    public string whichSnippetsSucceed(string snippet1, string snippet2,
        string evaluationResult)
    {
        return which(succeeds(snippet1, evaluationResult),
            succeeds(snippet2, evaluationResult));
    }

    public string whichSnippetsFail(string snippet1, string snippet2,
        string evaluationResult)
    {
        return which(fails(snippet1, evaluationResult),
            fails(snippet2, evaluationResult));
    }

    private static string which(bool b1, bool b2)
    {
        if (b1 && b2)
            return "both";

        if (b1)
            return "the first of";

        if (b2)
            return "the second of";

        return "neither";
    }

    private bool fails(string snippet, string evaluationResult)
    {
        return !succeeds(snippet, evaluationResult);
    }

    private bool succeeds(string snippet, string evaluationResult)
    {
        return new TestRig()
            .WithStubbedEvaluationResult(evaluationResult)
            .ProcessFragment(snippet)
            .IsSuccess;
    }

    public string normalize(string s)
    {
        // Bit naughty calling internal method normalize() directly
        return replaceRealWhitespaceCharactersWithNames(
            new DefaultExpectationChecker()
                .Normalize(replaceNamedWhitespaceWithRealWhitespaceCharacters(s)));
    }

    private static string replaceNamedWhitespaceWithRealWhitespaceCharacters(string s)
    {
        var spacesRemoved = Regex.Replace(s, "\\[SPACE\\]", " ");
        var tabsRemoved = Regex.Replace(spacesRemoved, "\\[TAB\\]", "\t");
        var lfRemoved = Regex.Replace(tabsRemoved, "\\[LF\\]", "\n");

        return Regex.Replace(lfRemoved, "\\[CR\\]", "\r");
    }

    private static string replaceRealWhitespaceCharactersWithNames(string s)
    {
        return s.Replace(" ", "[SPACE]");
    }
}
