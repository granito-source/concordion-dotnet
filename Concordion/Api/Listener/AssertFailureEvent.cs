namespace Concordion.Api.Listener;

public class AssertFailureEvent
{
    #region Properties

    public Element Element
    {
        get;
        private set;
    }

    public string Expected
    {
        get;
        private set;
    }

    public object Actual
    {
        get;
        private set;
    }

    #endregion

    #region Constructors

    public AssertFailureEvent(Element element, string expected, object actual) {
        Element = element;
        Expected = expected;
        Actual = actual;
    }

    #endregion
}
