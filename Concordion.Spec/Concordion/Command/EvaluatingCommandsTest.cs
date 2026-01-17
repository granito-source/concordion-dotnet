using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Concordion.NUnit;

namespace Concordion.Spec.Concordion.Command;

// [Ignore("address run command failures")]
// [ConcordionFixture]
[SuppressMessage("Performance", "CA1822")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class EvaluatingCommandsTest {
    private const string AssertEqualsResult = "assertequals";

    private const bool AssertTrueResult = true;

    private const bool AssertFalseResult = false;

    private static readonly IList<string> VerifyRowsResult = [
        "value1",
        "value2"
    ];

    private Exception? exceptionResult;

    public string ForAssertEquals()
    {
        return AssertEqualsResult;
    }

    public bool ForAssertTrue()
    {
        return AssertTrueResult;
    }

    public bool ForAssertFalse()
    {
        return AssertFalseResult;
    }

    public void ForException()
    {
        if (exceptionResult != null)
            throw exceptionResult;
    }

    public IEnumerable ForVerifyRows()
    {
        return VerifyRowsResult;
    }
}
