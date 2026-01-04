namespace Concordion.Api.Listener;

public class RunSuccessEvent
{
    #region Properties

    public Element Element { get; private set; }

    #endregion

    #region Constructors

    public RunSuccessEvent(Element element)
    {
        Element = element;
    }

    #endregion
}
