using System.Collections;
using Concordion.Integration;

namespace Concordion.Spec.Concordion.Command;

[ConcordionTest]
public class EvaluatingCommandsTest {
    public static string assertEqualsResult = "assertequals";

    public static bool assertTrueResult = true;

    public static bool assertFalseResult = false;

    public static Exception? exceptionResult;

    public static readonly IList<string> verifyRowsResult = ["value1", "value2"];

    public string ForAssertEquals()
    {
        return assertEqualsResult;
    }

    public bool ForAssertTrue()
    {
        return assertTrueResult;
    }

    public bool ForAssertFalse()
    {
        return assertFalseResult;
    }

    public void ForException()
    {
        if (exceptionResult != null)
            throw exceptionResult;
    }

    public IEnumerable ForVerifyRows()
    {
        return verifyRowsResult;
    }
}
