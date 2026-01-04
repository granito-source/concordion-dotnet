namespace Concordion.Api.Listener;

public class AssertSuccessEvent
{
    #region Properties

    public Element Element
    {
        get;
        private set;
    }

    #endregion

    #region Constructors

    public AssertSuccessEvent(Element element) {
        Element = element;
    }

    #endregion
}
