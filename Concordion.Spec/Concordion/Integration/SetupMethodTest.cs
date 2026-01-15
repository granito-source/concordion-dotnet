using Concordion.NUnit;
using NUnit.Framework;

namespace Concordion.Spec.Concordion.Integration;

// [Ignore("need to rework setup and teardown")]
// [ConcordionFixture]
public class SetupMethodTest {
    private List<string>? m_CalledMethods;

    private List<string> CalledMethods {
        get {
            if (m_CalledMethods == null)
                m_CalledMethods = [];

            return m_CalledMethods;
        }
    }

    [SetUp]
    public void Setup1()
    {
        CalledMethods.Add("Setup1");
    }

    [SetUp]
    public void Setup2()
    {
        CalledMethods.Add("Setup2");
    }

    public bool SetupMethodsCalled()
    {
        return CalledMethods.Count == 2 &&
            CalledMethods.Contains("Setup1") &&
            CalledMethods.Contains("Setup2");
    }
}
