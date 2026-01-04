namespace Concordion.Api.Listener;

public class MissingRowEvent
{
    #region Properties

    public Element RowElement { get; private set; }

    #endregion

    #region Constructors

    public MissingRowEvent(Element rowElement)
    {
        RowElement = rowElement;
    }

    #endregion
}
