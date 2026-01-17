namespace Concordion.Api.Listener;

public class AssertSuccessEvent(Element element) {
    public Element Element { get; } = element;
}
