namespace Concordion.Api.Listener;

public class ExecuteEvent
{
    #region Properties

    public Element Element { get; private set; }

    #endregion

    #region Constructors

    public ExecuteEvent(Element element)
    {
        Element = element;
    }

    #endregion
}
