namespace Concordion.Api.Listener;

public class AssertFailureEvent(Element element, string expected,
    object? actual) {
    public Element Element { get; } = element;

    public string Expected { get; } = expected;

    public object? Actual { get; } = actual;
}
