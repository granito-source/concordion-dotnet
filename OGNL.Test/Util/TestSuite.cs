using System.Collections;

namespace OGNL.Test.Util;

public class TestSuite {
    private ArrayList cases;

    public void addTest(OgnlTestCase testCase)
    {
        if (cases == null)
            cases = new ArrayList();

        cases.Add(testCase);
    }

    public IEnumerator enumerate()
    {
        if (cases == null)
            return null;

        return cases.GetEnumerator();
    }

    public OgnlTestCase this[int index] {
        get {
            if (cases == null)
                return null;

            return (OgnlTestCase)cases[index];
        }
    }
}

public interface ITestSuiteProvider {
    TestSuite suite();
}
