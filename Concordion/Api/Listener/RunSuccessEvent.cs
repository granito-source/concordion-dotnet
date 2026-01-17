namespace Concordion.Api.Listener;

public class RunSuccessEvent(Element element) {
    public Element Element { get; } = element;
}
