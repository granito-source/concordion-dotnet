namespace Concordion.Api.Listener;

public class RunFailureEvent
{
    #region Properties

    public Element Element { get; private set; }

    #endregion

    #region Constructors

    public RunFailureEvent(Element element)
    {
        Element = element;
    }

    #endregion
}
