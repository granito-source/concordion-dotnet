namespace Concordion.Api.Listener;

public class RunIgnoreEvent
{
    #region Properties

    public Element Element { get; private set; }

    #endregion

    #region Constructors

    public RunIgnoreEvent(Element element)
    {
        Element = element;
    }

    #endregion
}
