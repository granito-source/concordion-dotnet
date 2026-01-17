namespace Concordion.Api.Listener;

public class ExecuteEvent(Element element) {
    public Element Element { get; } = element;
}
