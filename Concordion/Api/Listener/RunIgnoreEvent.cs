namespace Concordion.Api.Listener;

public class RunIgnoreEvent(Element element) {
    public Element Element { get; } = element;
}
