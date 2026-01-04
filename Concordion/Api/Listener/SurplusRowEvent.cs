namespace Concordion.Api.Listener;

public class SurplusRowEvent
{
    #region Properties

    public Element RowElement { get; private set; }

    #endregion

    #region Constructors

    public SurplusRowEvent(Element rowElement)
    {
        RowElement = rowElement;
    }

    #endregion
}
