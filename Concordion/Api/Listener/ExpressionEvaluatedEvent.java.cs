namespace Concordion.Api.Listener;

public class ExpressionEvaluatedEvent
{
    #region Properties

    public Element Element { get; private set; }

    #endregion

    #region Constructors

    public ExpressionEvaluatedEvent(Element rowElement)
    {
        Element = rowElement;
    }

    #endregion
}
