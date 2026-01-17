namespace Concordion.Api.Listener;

public class RunFailureEvent(Element element) {
    public Element Element { get; } = element;
}
