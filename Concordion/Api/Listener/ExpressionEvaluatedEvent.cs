namespace Concordion.Api.Listener;

public class ExpressionEvaluatedEvent(Element rowElement) {
    public Element Element { get; } = rowElement;
}
